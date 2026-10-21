namespace DsGestor.Domain.Entities;

public class OcorrenciaRomaneioOcorrencia
{
    public int Id { get; set; }
    public int CodigoRomaneio { get; set; }
    public int CodigoOcorrencia { get; set; }

    public OcorrenciaRomaneio? Romaneio { get; set; }
    public Ocorrencia? Ocorrencia { get; set; }
}
