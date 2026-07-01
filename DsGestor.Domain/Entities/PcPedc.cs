namespace DsGestor.Domain.Entities;

public class PcPedc
{
    public long Numped { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;

    public decimal? Vltotal { get; set; }
    public int Codcli { get; set; }
    public int Codusur { get; set; }

    public DateTime? Dtentrega { get; set; }

    public decimal? Vltabela { get; set; }

    public string Codfilial { get; set; } = string.Empty;

    public decimal? Vldesconto { get; set; }

    public string? Tipovenda { get; set; }
    public string? Obs { get; set; }

    public decimal? Vlcustoreal { get; set; }
    public decimal? Vlcustofin { get; set; }
    public decimal? Vlfrete { get; set; }
    public decimal? Vloutrasdesp { get; set; }

    public decimal? Totpeso { get; set; }
    public decimal? Totvolume { get; set; }

    public int Codpraca { get; set; }

    public int? Numitens { get; set; }
    public int? Codemitente { get; set; }

    public DateTime? Dtcancel { get; set; }

    public string Posicao { get; set; } = "B";

    public decimal? Vlatend { get; set; }

    public string? Operacao { get; set; }

    public long? Numcar { get; set; }

    public string? Codcob { get; set; }

    public int? Hora { get; set; }
    public int? Minuto { get; set; }

    public long? Numseqentrega { get; set; }
    public decimal? Custoentrega { get; set; }

    public int Codsupervisor { get; set; }

    public string? Campanha { get; set; }
    public string? Numpedcli { get; set; }

    public int? Condvenda { get; set; }
    public decimal? Percvenda { get; set; }

    public string? Obs1 { get; set; }
    public string? Obs2 { get; set; }

    public decimal? Perdesc { get; set; }

    public string? Negociado { get; set; }

    public int Codplpag { get; set; }

    public int? Codfunccancel { get; set; }
    public long? Numtransvenda { get; set; }

    public string? Montando { get; set; }

    public long? Numpedrca { get; set; }

    public string? Fretdespacho { get; set; }
    public string? Freteredespacho { get; set; }

    public int? Codfornecfrete { get; set; }
    public string? Tipocarga { get; set; }

    public int? Prazo1 { get; set; }
    public int? Prazo2 { get; set; }
    public int? Prazo3 { get; set; }
    public int? Prazo4 { get; set; }
    public int? Prazo5 { get; set; }
    public int? Prazo6 { get; set; }
    public int? Prazo7 { get; set; }
    public int? Prazo8 { get; set; }
    public int? Prazo9 { get; set; }
    public int? Prazo10 { get; set; }
    public int? Prazo11 { get; set; }
    public int? Prazo12 { get; set; }

    public int? Prazomedio { get; set; }

    public string? Obsentrega1 { get; set; }
    public string? Obsentrega2 { get; set; }
    public string? Obsentrega3 { get; set; }

    public int? Codepto { get; set; }

    public string? Tipoembalagem { get; set; }

    public DateTime? Dtlibera { get; set; }

    public string? Codfilialnf { get; set; }

    public string? Origemped { get; set; }

    public string? Exportado { get; set; } = "N";

    public string? Importado { get; set; }

    public DateTime? Dtimportado { get; set; }

    public int? Numregiao { get; set; }

    public long? Numpedweb { get; set; }

    public string? Numtabella { get; set; }

    public string? SistemaLegado { get; set; }

    public string? NumPedMktPlace { get; set; }

    public string? RotinaLanc { get; set; }
}