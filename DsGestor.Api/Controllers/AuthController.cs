using DsGestor.Application.Dtos;
using DsGestor.Application.Services;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;

namespace DsGestor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUsuarioRepository _usuarioRepository;

    public AuthController(IAuthService authService, IUsuarioRepository usuarioRepository)
    {
        _authService = authService;
        _usuarioRepository = usuarioRepository;
    }

    // =========================
    // AUTH
    // =========================

    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] UserRegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("register-all")]
    public async Task<IActionResult> RegisterAll([FromBody] List<UserRegisterDto> usuarios)
    {
        try
        {
            if (usuarios == null || usuarios.Count == 0)
                return BadRequest(new { error = "Nenhum usuário informado para importação." });

            var importados = new List<object>();
            var falhas = new List<object>();

            foreach (var dto in usuarios)
            {
                try
                {
                    dto.Codigo = (dto.Codigo ?? "").Trim();
                    dto.Nome = (dto.Nome ?? "").Trim();
                    dto.Email = (dto.Email ?? "").Trim().ToLower();
                    dto.Senha = (dto.Senha ?? "").Trim();

                    // Força perfil vendedor para todos os usuários importados
                    dto.Perfil = PerfilUsuario.VENDEDOR;

                    // Se CodigoVendedor não vier preenchido, usa o próprio Codigo
                    if ((!dto.CodigoVendedor.HasValue || dto.CodigoVendedor.Value <= 0)
                        && int.TryParse(dto.Codigo, out var codigoVendedor))
                    {
                        dto.CodigoVendedor = codigoVendedor;
                    }

                    // Conforme seu padrão atual para essa importação
                    dto.CodigoSupervisor = 0;

                    var result = await _authService.RegisterAsync(dto);

                    importados.Add(new
                    {
                        dto.Codigo,
                        dto.Nome,
                        dto.Email,
                        dto.CodigoVendedor,
                        dto.CodigoSupervisor,
                        Perfil = dto.Perfil.ToString()
                    });
                }
                catch (Exception ex)
                {
                    falhas.Add(new
                    {
                        dto.Codigo,
                        dto.Nome,
                        dto.Email,
                        dto.CodigoVendedor,
                        Erro = ex.Message
                    });
                }
            }

            return Ok(new
            {
                totalRecebidos = usuarios.Count,
                totalImportados = importados.Count,
                totalFalhas = falhas.Count,
                importados,
                falhas
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDto>> Login([FromBody] UserLoginDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // =========================
    // ADMIN: USERS
    // Rotas:
    // GET    /api/auth/users
    // GET    /api/auth/users/{id}
    // PUT    /api/auth/users/{id}
    // =========================

    public class UserResumoVm
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public PerfilUsuario Perfil { get; set; }
        public int? CodigoVendedor { get; set; }
        public string? NomeVendedor { get; set; }
        public int? CodigoSupervisor { get; set; }
        public string? NomeSupervisor { get; set; }
    }

    public class UserUpdateVm
    {
        public string Codigo { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public PerfilUsuario Perfil { get; set; }
        public int? CodigoVendedor { get; set; }
        public int? CodigoSupervisor { get; set; }

        // ✅ opcional (se vier null/"" mantém)
        public string? Senha { get; set; }
    }

    //[Authorize(Roles = "Admin,Gerente")]
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserResumoVm>>> ListUsers()
    {
        try
        {
            var users = await _usuarioRepository.GetAllAsync();

            var result = users
                .OrderBy(u => u.Nome)
                .Select(u => new UserResumoVm
                {
                    Id = u.Id,
                    Codigo = u.Codigo ?? "",
                    Nome = u.Nome ?? "",
                    Email = u.Email ?? "",
                    Perfil = u.Perfil,
                    CodigoVendedor = u.CodigoVendedor,
                    CodigoSupervisor = u.CodigoSupervisor,
                    NomeVendedor = u.Vendedor?.Nome,
                    NomeSupervisor = u.Supervisor?.Nome
                });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    //[Authorize(Roles = "Admin,Gerente")]
    [HttpGet("users/{id:int}")]
    public async Task<ActionResult<UserResumoVm>> GetUserById(int id)
    {
        try
        {
            var u = await _usuarioRepository.GetByIdAsync(id);
            if (u == null) return NotFound(new { error = "Usuário não encontrado." });

            var result = new UserResumoVm
            {
                Id = u.Id,
                Codigo = u.Codigo ?? "",
                Nome = u.Nome ?? "",
                Email = u.Email ?? "",
                Perfil = u.Perfil,
                CodigoVendedor = u.CodigoVendedor,
                CodigoSupervisor = u.CodigoSupervisor,
                NomeVendedor = u.Vendedor?.Nome,
                NomeSupervisor = u.Supervisor?.Nome
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    //[Authorize(Roles = "Admin,Gerente")]
    [HttpPut("users/{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateVm dto)
    {
        try
        {
            var u = await _usuarioRepository.GetByIdAsync(id);
            if (u == null) return NotFound(new { error = "Usuário não encontrado." });

            var codigo = (dto.Codigo ?? "").Trim();
            var nome = (dto.Nome ?? "").Trim();
            var email = (dto.Email ?? "").Trim();

            if (string.IsNullOrWhiteSpace(codigo))
                return BadRequest(new { error = "Código é obrigatório." });

            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest(new { error = "Nome é obrigatório." });

            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { error = "Email é obrigatório." });

            // evita email duplicado
            var existingEmail = await _usuarioRepository.GetByEmailAsync(email);
            if (existingEmail != null && existingEmail.Id != id)
                return BadRequest(new { error = "Já existe usuário com esse e-mail." });

            // evita código duplicado
            var existingCodigo = await _usuarioRepository.GetByCodigoAsync(codigo);
            if (existingCodigo != null && existingCodigo.Id != id)
                return BadRequest(new { error = "Já existe usuário com esse código (matrícula)." });

            // aplica campos
            u.Codigo = codigo;
            u.Nome = nome;
            u.Email = email;
            u.Perfil = dto.Perfil;
            
            Console.WriteLine($"Perfil selecionado: {dto.Perfil}");

            // coerência vendedor/supervisor
            if (dto.Perfil == PerfilUsuario.VENDEDOR)
            {
            
                if (!dto.CodigoVendedor.HasValue || dto.CodigoVendedor.Value <= 0)
                    return BadRequest(new { error = "Para perfil Vendedor, informe CodigoVendedor." });

                u.CodigoVendedor = dto.CodigoVendedor;
                u.CodigoSupervisor = null;
            }
            else if (dto.Perfil == PerfilUsuario.SUPERVISOR)
            {
                if (!dto.CodigoSupervisor.HasValue || dto.CodigoSupervisor.Value <= 0)
                    return BadRequest(new { error = "Para perfil Supervisor, informe CodigoSupervisor." });

                u.CodigoSupervisor = dto.CodigoSupervisor;
                u.CodigoVendedor = null;
            }
            else
            {
                u.CodigoVendedor = 0;
                u.CodigoSupervisor = 0;
            }

            // ✅ senha opcional (usa o MESMO padrão do AuthService)
            var senha = (dto.Senha ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(senha))
            {
                PasswordHasher.ValidatePassword(senha);
                u.Pwd = PasswordHasher.HashPassword(senha);
            }

            await _usuarioRepository.UpdateAsync(u);

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}