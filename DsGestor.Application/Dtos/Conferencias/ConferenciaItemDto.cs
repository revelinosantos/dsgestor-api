namespace DsGestor.Application.DTOs.Conferencias;

public class ConferenciaItemDto
{
    public int Confid { get; set; }
    public decimal Numped { get; set; }
    public int Numnota { get; set; }

    public int Codprod { get; set; }
    public int Numseq { get; set; }
    public string? Descricao { get; set; }

    public string? Codauxiliar { get; set; }
    public string? Codauxiliar2 { get; set; }
    public string? Lpvcod { get; set; }
    public string? Lpvcodfind { get; set; }

    public int QtOriginal { get; set; }
    public int QtConferida { get; set; }
    public int QtFalta { get; set; }

    public decimal? Pvenda { get; set; }
    public decimal? PesoBrutoProduto { get; set; }
    public decimal? PesoLiquidoProduto { get; set; }

    public string? ConferidoItem { get; set; }
}