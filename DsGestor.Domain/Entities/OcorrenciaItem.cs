namespace DsGestor.Domain.Entities;

public class OcorrenciaItem
{
    public int Id { get; set; }
    public int CodigoOcorrencia { get; set; }
    public decimal NumeroTransacaoVenda { get; set; }

    // Identifica exatamente a linha original da nota
    public int NumeroItemNota { get; set; }

    public int CodigoProduto { get; set; }
    public decimal QuantidadeFaturada { get; set; }
    public decimal QuantidadeOcorrencia { get; set; }

    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }

    public int CodigoMotivo { get; set; }
    public string? Observacao { get; set; }

    public Ocorrencia? Ocorrencia { get; set; }
    public ProdutoOcorrencia? Produto { get; set; }
    public MotivoOcorrencia? Motivo { get; set; }
}
