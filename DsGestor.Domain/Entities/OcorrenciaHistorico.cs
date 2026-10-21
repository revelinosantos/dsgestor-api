using DsGestor.Domain.Enums;

namespace DsGestor.Domain.Entities;

public class OcorrenciaHistorico
{
    public int Id { get; set; }
    public int CodigoOcorrencia { get; set; }
    public DateTime Data { get; set; }

    public int CodigoUsuario { get; set; }
    public StatusOcorrencia? StatusAnterior { get; set; }
    public StatusOcorrencia StatusNovo { get; set; }

    public string? Observacao { get; set; }

    public Ocorrencia? Ocorrencia { get; set; }
    public Usuario? Usuario { get; set; }
}
