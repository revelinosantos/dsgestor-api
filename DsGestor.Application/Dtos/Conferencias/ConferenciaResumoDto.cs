namespace DsGestor.Application.DTOs.Conferencias;

public class ConferenciaResumoDto
{
    public decimal Numped { get; set; }
    public int Numnota { get; set; }
    public int? Codfilial { get; set; }
    public int? Codcli { get; set; }
    public string? Cliente { get; set; }
    public decimal? Vltotal { get; set; }
    public string? Statusconf { get; set; }

    public int TotalItens { get; set; }
    public int TotalItensConferidos { get; set; }
    public int TotalItensPendentes { get; set; }

    public int TotalUnidades { get; set; }
    public int TotalUnidadesConferidas { get; set; }
    public int TotalUnidadesPendentes { get; set; }

    public decimal PesoBrutoTotal { get; set; }
    public decimal PesoLiquidoTotal { get; set; }

    public DateTime? DataInicioConf { get; set; }
    public DateTime? DataFimConf { get; set; }
}