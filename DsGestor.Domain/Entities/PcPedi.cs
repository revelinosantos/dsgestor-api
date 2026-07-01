namespace DsGestor.Domain.Entities;

public class PcPedi
{
    public long Numped { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;

    public int Codcli { get; set; }
    public int Codprod { get; set; }
    public int Codusur { get; set; }

    public decimal Qt { get; set; }
    public decimal Pvenda { get; set; }
    public decimal Ptabela { get; set; }

    public long? Numcar { get; set; }

    public string Posicao { get; set; } = "B";

    public decimal St { get; set; }
    public decimal Vlcustofin { get; set; }
    public decimal Vlcustoreal { get; set; }

    public decimal Percom { get; set; }
    public decimal Perdesc { get; set; }

    public decimal? Qtfalta { get; set; }

    public int Numseq { get; set; }

    public string? Tipopeso { get; set; }

    public decimal? Percomtab { get; set; }
    public decimal? Perdesctab { get; set; }

    public int? Codmotnaocompra { get; set; }

    public decimal? Vldesccustocmv { get; set; }

    public decimal? Qtseparada { get; set; }
    public decimal? Qtvendaemb { get; set; }
    public decimal? Pvendaemb { get; set; }

    public decimal? Vloutros { get; set; }

    public decimal? Qtembalagem { get; set; }
    public decimal? Pvendaembalagem { get; set; }

    public long? Codauxiliar { get; set; }

    public decimal? Vlcustorep { get; set; }
    public decimal? Vlcustocont { get; set; }

    public int? Codcertific { get; set; }

    public decimal? Pvendabase { get; set; }

    public string? Nomeconcorrente { get; set; }

    public decimal? Preco { get; set; }

    public string? Prazo { get; set; }

    public decimal? Qtnaocompra { get; set; }

    public string? Codfilialretira { get; set; }

    public long? Numtira { get; set; }

    public int? Codfuncsep { get; set; }

    public decimal? Vldescsuframa { get; set; }

    public string? Numlote { get; set; }

    public decimal? Vldescrepasse { get; set; }

    public string? Refcor { get; set; }

    public int? Codfuncconf { get; set; }

    public DateTime? Dataconf { get; set; }

    public decimal? Vldescicmisencao { get; set; }

    public decimal? Qtoriginal { get; set; }

    public decimal? Vldescfornec { get; set; }
    public decimal? Vlfrete { get; set; }
    public decimal? Vlipi { get; set; }

    public decimal? Qtorig { get; set; }

    public decimal? Qtsepararun { get; set; }
    public decimal? Qtsepararcx { get; set; }

    public int? Codst { get; set; }

    public decimal? Vldescfin { get; set; }

    public decimal? Percipi { get; set; }
    public decimal? Iva { get; set; }

    public decimal? Aliqicms1 { get; set; }
    public decimal? Aliqicms2 { get; set; }

    public decimal? Pauta { get; set; }
    public decimal? Percbasered { get; set; }

    public decimal? Vldesccom { get; set; }

    public decimal? Perdesccom { get; set; }
    public decimal? Perdescfin { get; set; }

    public decimal? Vlbonific { get; set; }
    public decimal? Perbonific { get; set; }

    public decimal? Poriginal { get; set; }

    public decimal? Vlrebaixacmv { get; set; }

    public int? Numaplic { get; set; }

    public decimal? Perfretecmv { get; set; }

    public decimal? Vldescrodape { get; set; }

    public decimal? Stclientegnre { get; set; }

    public string? Imprime { get; set; }

    public string? Complemento { get; set; }

    public decimal? Custofinest { get; set; }

    public decimal? Percbaseredstfonte { get; set; }
    public decimal? Percbaseredst { get; set; }
    public decimal? Perdesccusto { get; set; }

    public decimal? Codicmtab { get; set; }
    public decimal? Txvenda { get; set; }

    public decimal? Percom2 { get; set; }
    public decimal? Percom3 { get; set; }

    public decimal? Perciss { get; set; }
    public decimal? Vliss { get; set; }

    public long? Numtranswms { get; set; }

    public string? Codpromocao { get; set; }

    public int? Prazomedio { get; set; }

    public string? Localizacao { get; set; }

    public decimal? Vlrepasse { get; set; }
    public decimal? Pbonific { get; set; }

    public decimal? Percvenda { get; set; }

    public decimal? Vldescpissuframa { get; set; }

    public int? Coddegustacao { get; set; }

    public decimal? Qtlocalizada { get; set; }

    public decimal? Perdescflex { get; set; }
    public decimal? Vldescflex { get; set; }

    public decimal? Perredcomiss { get; set; }
    public decimal? Vlredcomiss { get; set; }

    public string? Tipodescaplicado { get; set; }

    public decimal? Pbaserca { get; set; }

    public decimal? Pesobruto { get; set; }

    public int? Numverbarebcmv { get; set; }

    public int? Condvenda { get; set; }

    public int? Codplpag { get; set; }

    public long? Eancodprod { get; set; }

    public string? Brinde { get; set; }

    public decimal? Percomsup { get; set; }

    public decimal? Perredcomisssup { get; set; }
    public decimal? Vlredcomisssup { get; set; }

    public decimal? Baseicst { get; set; }

    public int? Numop { get; set; }

    public decimal? Qtcx { get; set; }
    public decimal? Qtpecas { get; set; }

    public int? Codfunclanc { get; set; }
    public int? Rotinalanc { get; set; }
    public DateTime? Dtlanc { get; set; }

    public int? Codfuncultalter { get; set; }
    public int? Rotinaultlalter { get; set; }
    public DateTime? Dtultlalter { get; set; }

    public int? Codsupervisor { get; set; }

    public string? Numpedcli { get; set; }
}