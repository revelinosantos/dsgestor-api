namespace DsGestor.Application.DTOs.Conferencias;

public class ConferirItemRequest
{
    public string Ean { get; set; } = string.Empty;
    public int Quantidade { get; set; } = 1;
    public int CodUsuario { get; set; }
}