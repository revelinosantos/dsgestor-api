namespace DsGestor.Domain.Entities;

public class MotivoOcorrencia
{
    public int Id { get; set; }
    public string Descricao { get; set; } = null!;
    public bool ExigeDevolucao { get; set; }
    public bool Ativo { get; set; }
    public int Ordem { get; set; }
    public DateTime DataCriacao { get; set; }

    public ICollection<OcorrenciaItem> Itens { get; set; }
        = new List<OcorrenciaItem>();
}
