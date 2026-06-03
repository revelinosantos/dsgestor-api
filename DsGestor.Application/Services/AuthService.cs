using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using DsGestor.Application.Dtos;
using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DsGestor.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

    // ========================= REGISTER =========================

    public async Task<UserResponseDto> RegisterAsync(UserRegisterDto dto)
    {
        var email = (dto.Email ?? "").Trim().ToLower();
        var codigo = (dto.Codigo ?? "").Trim();
        var nome = (dto.Nome ?? "").Trim();

        var existingUser = await _usuarioRepository.GetByEmailAsync(email);
        if (existingUser != null)
            throw new InvalidOperationException("Já existe um usuário com este e-mail.");

        PasswordHasher.ValidatePassword(dto.Senha);

        var hashedPwd = PasswordHasher.HashPassword(dto.Senha);

        var usuario = new Usuario
        {
            Codigo = codigo,
            Nome = nome,
            Email = email,
            Pwd = hashedPwd,
            Perfil = dto.Perfil,
            CodigoVendedor = dto.CodigoVendedor,
            CodigoSupervisor = dto.CodigoSupervisor
        };

        await _usuarioRepository.AddAsync(usuario);

        return GenerateUserResponse(usuario);
    }

    // ========================= LOGIN =========================

    public async Task<UserResponseDto> LoginAsync(UserLoginDto dto)
    {
        var email = (dto.Email ?? "").Trim().ToLower();

        var usuario = await _usuarioRepository.GetByEmailAsync(email);
        if (usuario == null)
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

        if (!PasswordHasher.VerifyPassword(dto.Senha, usuario.Pwd))
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

        return GenerateUserResponse(usuario);
    }

    // ========================= JWT TOKEN =========================

    private string GenerateJwtToken(Usuario usuario)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var expiresMinutes = int.Parse(jwtSection["ExpiresMinutes"] ?? "60");

        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var podeVisualizarPrecoMinimo = usuario.Perfil != PerfilUsuario.VENDEDOR;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Email),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),

            // Perfil usado como Role no Angular e na API
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),

            // Não vem do banco. É calculado pelo perfil.
            new Claim("visualizaPrecoMinimo", podeVisualizarPrecoMinimo.ToString().ToLower())
        };

        if (usuario.CodigoVendedor.HasValue)
            claims.Add(new Claim("codigoVendedor", usuario.CodigoVendedor.Value.ToString()));

        if (usuario.CodigoSupervisor.HasValue)
            claims.Add(new Claim("codigoSupervisor", usuario.CodigoSupervisor.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ========================= RESPONSE =========================

    private UserResponseDto GenerateUserResponse(Usuario usuario)
    {
        var token = GenerateJwtToken(usuario);

        return new UserResponseDto
        {
            Id = usuario.Id,
            Codigo = usuario.Codigo,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            CodigoSupervisor = usuario.CodigoSupervisor,
            CodigoVendedor = usuario.CodigoVendedor,
            Token = token
        };
    }
}