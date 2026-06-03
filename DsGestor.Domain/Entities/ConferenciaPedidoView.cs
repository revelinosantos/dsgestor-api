namespace DsGestor.Domain.Entities;

public class ConferenciaPedidoView
{
    public decimal Numped { get; set; }
    public DateTime? Dataped { get; set; }
    public string? Statusped { get; set; }
    public int? Codfilial { get; set; }
    public int? Codusur { get; set; }
    public int? Condvenda { get; set; }
    public int? Codemitenteped { get; set; }
    public int? Codcli { get; set; }
    public string? Cliente { get; set; }
    public decimal? Vltotal { get; set; }

    public int Codprod { get; set; }
    public int Numseq { get; set; }
    public string? Descricao { get; set; }

    public int? Qtoriginal { get; set; }
    public int? Qt { get; set; }
    public int? Qtfalta { get; set; }
    public int? Qtunit { get; set; }
    public int? Qtunitcx { get; set; }

    public decimal? Pvenda { get; set; }
    public string? Codauxiliar { get; set; }

    public decimal? Pesobrutopro { get; set; }
    public decimal? Pesoliqpro { get; set; }

    public int? Codconf { get; set; }
    public int? Volume { get; set; }

    public decimal? Pesobrutoped { get; set; }
    public decimal? Pesoliqped { get; set; }

    public int? Codsep { get; set; }
    public string? Nomeconf { get; set; }

    public string? Codauxiliar2 { get; set; }

    public int? Numnota { get; set; }
    public int? Numtransvenda { get; set; }

    public int? Lpvid { get; set; }
    public string? Lpvcod { get; set; }
    public string? Lpvcodfind { get; set; }
}