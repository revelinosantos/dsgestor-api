namespace DsGestor.Domain.Entities;

public class NotaFiscalOcorrenciaView
{
    public DateTime DataSaida { get; set; }
    public string CodigoFilial { get; set; } = null!;
    public string? CodigoFilialNf { get; set; }

    public decimal NumeroNota { get; set; }
    public string Serie { get; set; } = null!;
    public decimal NumeroTransacaoVenda { get; set; }

    public int CodigoCliente { get; set; }
    public string Cliente { get; set; } = null!;
    public string? Fantasia { get; set; }
    public string? Cidade { get; set; }

    public int? CodigoVendedor { get; set; }
    public string? Vendedor { get; set; }

    public int? CodigoMotorista { get; set; }
    public string? Motorista { get; set; }

    public int NumeroTransacaoItem { get; set; }
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = null!;
    public string? Unidade { get; set; }

    public decimal Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotalItem { get; set; }

}
