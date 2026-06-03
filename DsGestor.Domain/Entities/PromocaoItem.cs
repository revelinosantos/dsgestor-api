using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DsGestor.Domain.Entities;
public class PromocaoItem
{
    public long Id { get; set; }

    public int CodigoCliente { get; set; }
    public int CodigoProduto { get; set; }
    public Decimal? PercDesc { get; set; }
    public DateTime DtInicio { get; set; }
    public DateTime DtFim { get; set; }
    public int CodFuncLanc { get; set; }
    public DateTime DataLanc { get; set; } = DateTime.Now;
    public int CodFuncUltAlter { get; set; }
    public DateTime DataUltAlter { get; set; } = DateTime.Now;
    public string AplicaDesconto { get; set; } = "S";
    public string BaseCredDebBrca { get; set; } = "N";
    public string UtilizaDescRede { get; set; } = "N";
    public string CreditaSobrePolitica { get; set; } = "N";
    public string Tipo { get; set; } = "C";
    public string OrigemPed { get; set; } = "T";
    public int CodigoPromocao { get; set; }
    public string TipoPoliticaPromocaoMed { get; set; } = "D";
    public string SyncFv { get; set; } = "N";
    public string IonSync { get; set; } = "N";
    public string? Descricao { get; set; }
    public string? GrupoValidacao { get; set; }
}