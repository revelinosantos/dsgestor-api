namespace DsGestor.Application.DTOs.Conferencias;

public class FinalizarDivergenciaRequest
{
    public int CodUsuario { get; set; }
    public int CodSupervisor { get; set; }
    public string SenhaSupervisor { get; set; } = string.Empty;
    public string? Observacao { get; set; }
}