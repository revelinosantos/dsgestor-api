using System.Text.Json.Serialization;

namespace DsGestor.Application.Dtos.CoteFacil;

public sealed class CoteFacilResponseConteudo<T>
{
    [JsonPropertyName("cnpjDistribuidor")]
    public string CnpjDistribuidor { get; set; } = string.Empty;

    [JsonPropertyName("conteudo")]
    public List<T> Conteudo { get; set; } = new();

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("totalElements")]
    public int TotalElements { get; set; }
}

public sealed class CoteFacilProdutoRequest
{
    [JsonPropertyName("cnpjDistribuidor")]
    public string? CnpjDistribuidor { get; set; }

    [JsonPropertyName("produto")]
    public CoteFacilProdutoFiltroDto? Produto { get; set; }
}

public sealed class CoteFacilProdutoFiltroDto
{
    [JsonPropertyName("codigoProduto")]
    public string? CodigoProduto { get; set; }

    [JsonPropertyName("EAN")]
    public string? Ean { get; set; }

    [JsonPropertyName("DUN")]
    public string? Dun { get; set; }

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }
}

public sealed class CoteFacilProdutoCatalogoDto
{
    [JsonPropertyName("cnpjFabricante")]
    public string? CnpjFabricante { get; set; }

    [JsonPropertyName("fabricante")]
    public string? Fabricante { get; set; }

    [JsonPropertyName("cnpjFornecedor")]
    public string? CnpjFornecedor { get; set; }

    [JsonPropertyName("nomeFornecedor")]
    public string? NomeFornecedor { get; set; }

    [JsonPropertyName("ativo")]
    public string Ativo { get; set; } = "S";

    [JsonPropertyName("codigoProduto")]
    public string CodigoProduto { get; set; } = string.Empty;

    [JsonPropertyName("tipoProduto")]
    public string? TipoProduto { get; set; }

    [JsonPropertyName("NCM")]
    public string? Ncm { get; set; }

    [JsonPropertyName("EAN")]
    public string? Ean { get; set; }

    [JsonPropertyName("DUN")]
    public string? Dun { get; set; }

    [JsonPropertyName("embalagemEan")]
    public string? EmbalagemEan { get; set; }

    [JsonPropertyName("qtdeEan")]
    public decimal? QtdeEan { get; set; }

    [JsonPropertyName("embalagemDun")]
    public string? EmbalagemDun { get; set; }

    [JsonPropertyName("qtdeDUN")]
    public decimal? QtdeDun { get; set; }

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }

    [JsonPropertyName("categoria1")]
    public string? Categoria1 { get; set; }

    [JsonPropertyName("categoria2")]
    public string? Categoria2 { get; set; }

    [JsonPropertyName("categoria3")]
    public string? Categoria3 { get; set; }

    [JsonPropertyName("imagem")]
    public string? Imagem { get; set; }

    [JsonPropertyName("observacao")]
    public string? Observacao { get; set; }
}

// =============================
// 8. Consulta de preço
// =============================

public sealed class CoteFacilConsultaPrecoRequest
{
    [JsonPropertyName("cliente")]
    public CoteFacilPrecoClienteDto? Cliente { get; set; }

    [JsonPropertyName("pagamento")]
    public CoteFacilConsultaPrecoPagamentoDto? Pagamento { get; set; }

    [JsonPropertyName("produto")]
    public List<CoteFacilProdutoFiltroDto> Produto { get; set; } = new();
}

public sealed class CoteFacilPrecoClienteDto
{
    [JsonPropertyName("cnpjDistribuidor")]
    public string? CnpjDistribuidor { get; set; }

    [JsonPropertyName("cnpjCliente")]
    public string? CnpjCliente { get; set; }
}

public sealed class CoteFacilConsultaPrecoPagamentoDto
{
    [JsonPropertyName("cotacaoCoteFacil")]
    public long? CotacaoCoteFacil { get; set; }

    [JsonPropertyName("codigoCondicaoComercial")]
    public int? CodigoCondicaoComercial { get; set; }

    [JsonPropertyName("codigoPrazoPagamento")]
    public int? CodigoPrazoPagamento { get; set; }
}

public sealed class CoteFacilPrecoDto
{
    [JsonPropertyName("codigoProduto")]
    public string CodigoProduto { get; set; } = string.Empty;

    [JsonPropertyName("EAN")]
    public string? Ean { get; set; }

    [JsonPropertyName("DUN")]
    public string? Dun { get; set; }

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }

    [JsonPropertyName("valorFabrica")]
    public decimal ValorFabrica { get; set; }

    [JsonPropertyName("descontoComercial")]
    public decimal DescontoComercial { get; set; }

    [JsonPropertyName("valorDescontoComercial")]
    public decimal ValorDescontoComercial { get; set; }

    [JsonPropertyName("valorUnitario")]
    public decimal ValorUnitario { get; set; }

    [JsonPropertyName("valorUnitarioNF")]
    public decimal ValorUnitarioNf { get; set; }

    [JsonPropertyName("valorUnitarioBoleto")]
    public decimal ValorUnitarioBoleto { get; set; }
}

// =============================
// 12. Estoque produto
// =============================

public sealed class CoteFacilEstoqueProdutoRequest
{
    [JsonPropertyName("cnpjDistribuidor")]
    public string? CnpjDistribuidor { get; set; }

    [JsonPropertyName("produto")]
    public CoteFacilProdutoFiltroDto? Produto { get; set; }
}

public sealed class CoteFacilEstoqueProdutoDto
{
    [JsonPropertyName("codigoProduto")]
    public string CodigoProduto { get; set; } = string.Empty;

    [JsonPropertyName("EAN")]
    public string? Ean { get; set; }

    [JsonPropertyName("DUN")]
    public string? Dun { get; set; }

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }

    [JsonPropertyName("estoque")]
    public decimal Estoque { get; set; }

    [JsonPropertyName("estoqueDisponivel")]
    public decimal EstoqueDisponivel { get; set; }

    [JsonPropertyName("codigoFilial")]
    public string? CodigoFilial { get; set; }
}