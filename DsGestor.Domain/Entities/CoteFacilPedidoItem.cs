namespace DsGestor.Domain.Entities;

public class CoteFacilPedidoItem
{
    public long IdItem { get; set; }
    public long IdPedido { get; set; }
    public int Sequencia { get; set; }

    public string? Ean { get; set; }
    public string? Dun { get; set; }
    public string? CodigoProdutoCoteFacil { get; set; }
    public int? CodprodWinthor { get; set; }

    public string? DescricaoProduto { get; set; }
    public string? Embalagem { get; set; }
    public string? Unidade { get; set; }

    public decimal QuantidadeSolicitada { get; set; }
    public decimal? QuantidadeAtendida { get; set; }

    public decimal? ValorFabrica { get; set; }
    public decimal? ValorUnitario { get; set; }
    public decimal? ValorUnitarioNf { get; set; }
    public decimal? ValorUnitarioBoleto { get; set; }
    public decimal? ValorTotalItem { get; set; }

    public decimal? DescontoAdicional { get; set; }
    public decimal? ValorDescAdicional { get; set; }

    public decimal? DescontoBonificacao { get; set; }
    public decimal? ValorDescBonificacao { get; set; }

    public decimal? DescontoComercial { get; set; }
    public decimal? ValorDescComercial { get; set; }

    public decimal? DescontoFinanceiro { get; set; }
    public decimal? ValorDescFinanceiro { get; set; }

    public long? CodigoPromocao { get; set; }

    public decimal? ValorUnitarioCalculado { get; set; }
    public decimal? EstoqueDisponivel { get; set; }

    public string Status { get; set; } = "RECEBIDO";
    public string? MensagemErro { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime? DataAtualizacao { get; set; }

    public CoteFacilPedido? Pedido { get; set; }
}