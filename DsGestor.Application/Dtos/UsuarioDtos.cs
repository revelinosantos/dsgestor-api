using DsGestor.Domain.Enums;

namespace DsGestor.Application.Dtos.Usuarios;

public class UsuarioResumoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public PerfilUsuario Perfil { get; set; }
    public int? CodigoVendedor { get; set; }
    public int? CodigoSupervisor { get; set; }
}

public class UsuarioDetalheDto : UsuarioResumoDto
{
}

public class UsuarioUpdateDto
{
    public string Codigo { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public PerfilUsuario Perfil { get; set; }
    public int? CodigoVendedor { get; set; }
    public int? CodigoSupervisor { get; set; }

    /// <summary>
    /// Se vier null/"" mantém a senha atual.
    /// Se vier preenchida, atualiza (lembre de aplicar o mesmo hash do Auth).
    /// </summary>
    public string? Senha { get; set; }
}
