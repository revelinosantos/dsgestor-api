using DsGestor.Domain.Enums;

namespace DsGestor.Domain.Entities;

public class OcorrenciaRomaneio
{
    public int Id { get; set; }
    public DateTime Data { get; set; }

    public string CodigoFilial { get; set; } = null!;
    public int CodigoMotorista { get; set; }
    public int CodigoUsuario { get; set; }

    public StatusRomaneioOcorrencia Status { get; set; }
    public string? Observacao { get; set; }

    public DateTime? DataCancelamento { get; set; }
    public int? CodigoUsuarioCancelamento { get; set; }
    public string? MotivoCancelamento { get; set; }

    public Motorista? Motorista { get; set; }
    public Usuario? Usuario { get; set; }
    public Usuario? UsuarioCancelamento { get; set; }

    public ICollection<OcorrenciaRomaneioOcorrencia> Ocorrencias { get; set; }
        = new List<OcorrenciaRomaneioOcorrencia>();
}
