namespace DsGestor.Domain.Entities;

public class ProdutoOcorrencia
{
    public int Id { get; set; }
    public string Descricao { get; set; } = null!;
    public string? Unidade { get; set; }
}