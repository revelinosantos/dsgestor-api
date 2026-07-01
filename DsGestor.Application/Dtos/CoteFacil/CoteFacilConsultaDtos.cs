using System.Text.Json.Serialization;

namespace DsGestor.Application.Dtos.CoteFacil;

public sealed class CoteFacilPagedResponse<T>
{
    [JsonPropertyName("sucesso")]
    public bool Sucesso { get; set; }

    [JsonPropertyName("mensagem")]
    public string Mensagem { get; set; } = string.Empty;

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("totalElements")]
    public int TotalElements { get; set; }

    [JsonPropertyName("content")]
    public List<T> Content { get; set; } = new();
}

// =============================
// 2. Filiais do Distribuidor
// =============================

public sealed class CoteFacilFilialDistribuidorDto
{
    [JsonPropertyName("codigoFilial")]
    public string CodigoFilial { get; set; } = string.Empty;

    [JsonPropertyName("cnpjDistribuidor")]
    public string CnpjDistribuidor { get; set; } = string.Empty;

    [JsonPropertyName("razaoSocial")]
    public string? RazaoSocial { get; set; }

    [JsonPropertyName("nomeFantasia")]
    public string? NomeFantasia { get; set; }

    [JsonPropertyName("cidade")]
    public string? Cidade { get; set; }

    [JsonPropertyName("uf")]
    public string? Uf { get; set; }
}

// =============================
// 3. Cliente
// =============================

public sealed class CoteFacilClienteRequest
{
    [JsonPropertyName("cnpjDistribuidor")]
    public string? CnpjDistribuidor { get; set; }

    [JsonPropertyName("cnpjCliente")]
    public string? CnpjCliente { get; set; }
}

public sealed class CoteFacilClienteDto
{
    [JsonPropertyName("codigoCliente")]
    public int CodigoCliente { get; set; }

    [JsonPropertyName("cnpjCliente")]
    public string CnpjCliente { get; set; } = string.Empty;

    [JsonPropertyName("razaoSocial")]
    public string? RazaoSocial { get; set; }

    [JsonPropertyName("nomeFantasia")]
    public string? NomeFantasia { get; set; }

    [JsonPropertyName("endereco")]
    public string? Endereco { get; set; }

    [JsonPropertyName("bairro")]
    public string? Bairro { get; set; }

    [JsonPropertyName("cidade")]
    public string? Cidade { get; set; }

    [JsonPropertyName("uf")]
    public string? Uf { get; set; }

    [JsonPropertyName("telefone")]
    public string? Telefone { get; set; }

    [JsonPropertyName("codigoFilial")]
    public string? CodigoFilial { get; set; }

    [JsonPropertyName("codigoVendedor")]
    public int? CodigoVendedor { get; set; }

    [JsonPropertyName("nomeVendedor")]
    public string? NomeVendedor { get; set; }

    [JsonPropertyName("numRegiao")]
    public int? NumRegiao { get; set; }

    [JsonPropertyName("nomeRegiao")]
    public string? NomeRegiao { get; set; }

    [JsonPropertyName("limiteCredito")]
    public decimal? LimiteCredito { get; set; }
}

// =============================
// 7. Condição de pagamento
// =============================

public sealed class CoteFacilCondicaoPagamentoRequest
{
    [JsonPropertyName("cnpjDistribuidor")]
    public string? CnpjDistribuidor { get; set; }

    [JsonPropertyName("codigoCondicaoPagamento")]
    public int? CodigoCondicaoPagamento { get; set; }

    [JsonPropertyName("condicaoPagamento")]
    public CoteFacilCondicaoPagamentoFiltroDto? CondicaoPagamento { get; set; }
}

public sealed class CoteFacilCondicaoPagamentoFiltroDto
{
    [JsonPropertyName("codigoCondicaoPagamento")]
    public int? CodigoCondicaoPagamento { get; set; }

    [JsonPropertyName("codigoPrazoPagamento")]
    public int? CodigoPrazoPagamento { get; set; }

    [JsonPropertyName("codigoCondicaoComercial")]
    public int? CodigoCondicaoComercial { get; set; }
}

public sealed class CoteFacilCondicaoPagamentoDto
{
    [JsonPropertyName("codigoCondicaoPagamento")]
    public int CodigoCondicaoPagamento { get; set; }

    [JsonPropertyName("codigoPrazoPagamento")]
    public int CodigoPrazoPagamento { get; set; }

    [JsonPropertyName("codigoCondicaoComercial")]
    public int CodigoCondicaoComercial { get; set; }

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }

    [JsonPropertyName("numDias")]
    public int? NumDias { get; set; }
}