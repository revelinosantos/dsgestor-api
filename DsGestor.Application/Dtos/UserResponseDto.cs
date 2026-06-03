using DsGestor.Domain.Enums;

namespace DsGestor.Application.Dtos;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public PerfilUsuario Perfil { get; set; }
    public int? CodigoVendedor { get; set; }
    public int? CodigoSupervisor { get; set; }
    public string Token { get; set; } = null!;
}
