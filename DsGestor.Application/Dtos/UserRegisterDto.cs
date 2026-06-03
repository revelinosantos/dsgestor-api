using DsGestor.Domain.Enums;

namespace DsGestor.Application.Dtos
{
public class UserRegisterDto
{
    public string Codigo { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public PerfilUsuario Perfil { get; set; }
    public int? CodigoVendedor { get; set; }
    public int? CodigoSupervisor { get; set; }
}
}