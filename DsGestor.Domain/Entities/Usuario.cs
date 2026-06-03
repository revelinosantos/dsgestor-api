using DsGestor.Domain.Enums;

namespace DsGestor.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Pwd { get; set; } = null!; 
    public PerfilUsuario Perfil { get; set; }
    public int? CodigoVendedor { get; set; }
    public int? CodigoSupervisor { get; set; }
    public Vendedor? Vendedor { get; set; }
    public Supervisor? Supervisor { get; set; }


}
