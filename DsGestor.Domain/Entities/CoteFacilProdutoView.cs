namespace DsGestor.Domain.Entities;

public class CoteFacilProdutoView
{
    public string? Codfilial { get; set; }
    public string CnpjDistribuidor { get; set; } = string.Empty;
    public string? RazaoSocialDistribuidor { get; set; }
    public string? FantasiaDistribuidor { get; set; }

    public int CodigoProduto { get; set; }
    public string? Ean { get; set; }
    public string? Dun { get; set; }
    public string? Descricao { get; set; }
    public string? Unidade { get; set; }

    public string? EmbalagemEan { get; set; }
    public decimal? QtdeEan { get; set; }
    public string? EmbalagemDun { get; set; }
    public decimal? QtdeDun { get; set; }

    public string? TipoProduto { get; set; }
    public string? Ncm { get; set; }

    public string? CnpjFornecedor { get; set; }
    public string? NomeFornecedor { get; set; }

    public string? CnpjFabricante { get; set; }
    public string? Fabricante { get; set; }

    public string? Departamento { get; set; }
    public string? Categoria1 { get; set; }
    public string? Categoria2 { get; set; }
    public string? Categoria3 { get; set; }

    public int? NumRegiao { get; set; }

    public decimal? PrecoFabrica { get; set; }
    public decimal? DescontoMaximo { get; set; }
    public decimal? PrecoVenda { get; set; }

    public decimal? EstoqueGeral { get; set; }
    public decimal? EstoqueReservado { get; set; }
    public decimal? EstoqueBloqueado { get; set; }
    public decimal? EstoqueDisponivel { get; set; }

    public string? Imagem { get; set; }
    public string? Ativo { get; set; }
    public string? Observacao { get; set; }
}