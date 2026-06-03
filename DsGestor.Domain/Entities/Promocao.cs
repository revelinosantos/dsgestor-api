namespace DsGestor.Domain.Entities;
public class Promocao
{
    public int Id { get; set; }
    public string IdentificadorPromocao { get; set; } = "PRMCON";
    public string TipoPolitica { get; set; } = "D";
    public string DescricaoResumida { get; set; } = "";
    public string DescricaoDetalhada { get; set; } = "";
    public DateTime DataInicial { get; set; }
    public DateTime DataFinal { get; set; }
    public string TipoPromocao { get; set; } = "C";
    public int TipoRestricao { get; set; } = 1;
    public int CodFuncLanc { get; set; }
    public DateTime DataLanc { get; set; } = DateTime.Now;
    public string AlteraDesconto { get; set; } = "S";
    public string SyncFv { get; set; } = "N";
    public string CodReferencia { get; set; } = "";
    public string BaseCredDebBrca { get; set; } = "N";
    public string CreditaSobrePolitica { get; set; } = "N";
    public string ConsideraCalcGiroMedic { get; set; } = "S";
    public string UtilizaDescRede { get; set; } = "N";
    public string TipoPlanoPag { get; set; } = "1";
    public string AplicaFamiliaProdutos { get; set; } = "N";
    public string TipoSimplesNacional { get; set; } = "3";
    public string PontuarAcordoParceria { get; set; } = "S";
    public string PrazoPlPagMandatorio { get; set; } = "N";
    public string Bloqueio { get; set; } = "N";
    public string UtilizaPercDescBaseRca { get; set; } = "N";
    public string OrigemPreco { get; set; } = "T";
    public string VisualizarPromoEsp { get; set; } = "N";
    public string UsaNormalizacaoDesc { get; set; } = "S";
    public string ParticipaPremiacaoCli { get; set; } = "S";
    public string MultiplosBrindes { get; set; } = "N";
    public string AplicaDesconto { get; set; } = "S";
    public string TipoSolicitante { get; set; } = "F";
    public string AceitaDescPromCampanha { get; set; } = "N";
    public string ValidarMultEmbMaster { get; set; } = "N";
    public string ParticipaPremiacaoInd { get; set; } = "S";
    public string IncluirFiltroTelevendas { get; set; } = "N";
    public int TipoCotaGeral { get; set; } = 0;
    public int TipoCotaCliente { get; set; } = 0;
    public string IonSync { get; set; } = "N";
}