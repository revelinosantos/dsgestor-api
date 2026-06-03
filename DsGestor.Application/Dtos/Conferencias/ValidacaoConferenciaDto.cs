namespace DsGestor.Application.DTOs.Conferencias;

public class ValidacaoConferenciaDto
{
    public decimal Numped { get; set; }
    public int Numnota { get; set; }

    public bool ConferenciaCompleta { get; set; }

    public int TotalItens { get; set; }
    public int ItensConferidos { get; set; }
    public int ItensPendentes { get; set; }

    public int TotalUnidades { get; set; }
    public int TotalUnidadesConferidas { get; set; }
    public int TotalUnidadesPendentes { get; set; }

    public List<ConferenciaItemDto> Faltantes { get; set; } = new();
}