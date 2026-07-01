using System.Text.Json.Serialization;

namespace DsGestor.Application.Dtos.CoteFacil;

public sealed class CoteFacilInserirPedidoRequest
{
    [JsonPropertyName("distribuidor")]
    public CoteFacilDistribuidorDto? Distribuidor { get; set; }

    [JsonPropertyName("cliente")]
    public CoteFacilClientePedidoDto? Cliente { get; set; }

    [JsonPropertyName("cotefacil")]
    public CoteFacilOrigemPedidoDto? Cotefacil { get; set; }

    [JsonPropertyName("promocao")]
    public CoteFacilPromocaoDto? Promocao { get; set; }

    [JsonPropertyName("pagamento")]
    public CoteFacilPagamentoDto? Pagamento { get; set; }

    [JsonPropertyName("produtos")]
    public List<CoteFacilProdutoPedidoDto> Produtos { get; set; } = new();
}

public sealed class CoteFacilDistribuidorDto
{
    [JsonPropertyName("cnpjDistribuidor")]
    public string? CnpjDistribuidor { get; set; }

    [JsonPropertyName("codigoDistribuidor")]
    public int? CodigoDistribuidor { get; set; }
}

public sealed class CoteFacilClientePedidoDto
{
    [JsonPropertyName("codigoCliente")]
    public int? CodigoCliente { get; set; }

    [JsonPropertyName("cnpjCliente")]
    public string? CnpjCliente { get; set; }
}

public sealed class CoteFacilOrigemPedidoDto
{
    [JsonPropertyName("cotacaoCoteFacil")]
    public long? CotacaoCoteFacil { get; set; }

    [JsonPropertyName("pedidoCoteFacil")]
    public long? PedidoCoteFacil { get; set; }

    [JsonPropertyName("pedidoCliente")]
    public string? PedidoCliente { get; set; }
}

public sealed class CoteFacilPromocaoDto
{
    [JsonPropertyName("codigoPromocao")]
    public long? CodigoPromocao { get; set; }
}

public sealed class CoteFacilPagamentoDto
{
    [JsonPropertyName("codigoPrazoPagamento")]
    public int? CodigoPrazoPagamento { get; set; }

    [JsonPropertyName("codigoCondicaoComercial")]
    public int? CodigoCondicaoComercial { get; set; }
}

public sealed class CoteFacilProdutoPedidoDto
{
    [JsonPropertyName("EAN")]
    public string? Ean { get; set; }

    [JsonPropertyName("DUN")]
    public string? Dun { get; set; }

    [JsonPropertyName("codigoProduto")]
    public string? CodigoProduto { get; set; }

    [JsonPropertyName("qtdSolicitada")]
    public decimal? QuantidadeSolicitada { get; set; }

    [JsonPropertyName("descontoAdicional")]
    public decimal? DescontoAdicional { get; set; }

    [JsonPropertyName("valorDescontoAdicional")]
    public decimal? ValorDescontoAdicional { get; set; }

    [JsonPropertyName("descontoBonificacao")]
    public decimal? DescontoBonificacao { get; set; }

    [JsonPropertyName("valorDescontoBonificacao")]
    public decimal? ValorDescontoBonificacao { get; set; }

    [JsonPropertyName("descontoComercial")]
    public decimal? DescontoComercial { get; set; }

    [JsonPropertyName("valorDescontoComercial")]
    public decimal? ValorDescontoComercial { get; set; }

    [JsonPropertyName("descontoFinanceiro")]
    public decimal? DescontoFinanceiro { get; set; }

    [JsonPropertyName("valorDescontoFinanceiro")]
    public decimal? ValorDescontoFinanceiro { get; set; }

    [JsonPropertyName("codigoPromocao")]
    public long? CodigoPromocao { get; set; }

    [JsonPropertyName("valorFabrica")]
    public decimal? ValorFabrica { get; set; }

    [JsonPropertyName("valorUnitario")]
    public decimal? ValorUnitario { get; set; }

    [JsonPropertyName("valorUnitarioNF")]
    public decimal? ValorUnitarioNf { get; set; }

    [JsonPropertyName("valorUnitarioBoleto")]
    public decimal? ValorUnitarioBoleto { get; set; }
}

public sealed class CoteFacilInserirPedidoResponse
{
    public bool Sucesso { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public long? IdPedido { get; set; }
    public long? PedidoCoteFacil { get; set; }
    public long? CotacaoCoteFacil { get; set; }
    public long? NumPedWinThor { get; set; }
}

public sealed class CoteFacilConfirmacaoPedidoRequest
{
    [JsonPropertyName("cnpjDistribuidor")]
    public string? CnpjDistribuidor { get; set; }

    [JsonPropertyName("dataInicial")]
    public string? DataInicial { get; set; }

    [JsonPropertyName("dataFinal")]
    public string? DataFinal { get; set; }

    [JsonPropertyName("pedidos")]
    public List<string> Pedidos { get; set; } = new();
}

public sealed class CoteFacilConfirmacaoPedidoResponse
{
    public bool Sucesso { get; set; }
    public List<CoteFacilPedidoConfirmadoDto> Pedidos { get; set; } = new();
}

public sealed class CoteFacilPedidoConfirmadoDto
{
    public long IdPedido { get; set; }
    public long? PedidoCoteFacil { get; set; }
    public long? CotacaoCoteFacil { get; set; }
    public string? PedidoCliente { get; set; }
    public string? CnpjCliente { get; set; }
    public string Status { get; set; } = string.Empty;
    public long? NumPedWinThor { get; set; }
    public string? MensagemErro { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataProcessamento { get; set; }
}