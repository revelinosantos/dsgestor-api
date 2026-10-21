using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;

namespace DsGestor.Domain.Repositories;

public interface IOcorrenciaRomaneioRepository
{
    Task<IEnumerable<OcorrenciaRomaneio>> GetAllAsync(
        string? codigoFilial = null,
        int? codigoMotorista = null,
        StatusRomaneioOcorrencia? status = null,
        CancellationToken ct = default);

    Task<OcorrenciaRomaneio?> GetByIdAsync(
        int id,
        CancellationToken ct = default);

    Task<IEnumerable<Ocorrencia>> GetOcorrenciasPendentesAsync(
        int codigoMotorista,
        string? codigoFilial = null,
        CancellationToken ct = default);

    Task<OcorrenciaRomaneio> CreateAsync(
        OcorrenciaRomaneio romaneio,
        IReadOnlyCollection<int> codigosOcorrencias,
        CancellationToken ct = default);

    Task CancelarAsync(
        int id,
        int codigoUsuario,
        string motivo,
        CancellationToken ct = default);
}
