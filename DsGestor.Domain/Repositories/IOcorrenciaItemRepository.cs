using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface IOcorrenciaItemRepository
{
    Task<IEnumerable<OcorrenciaItem>> GetByOcorrenciaAsync(int codigoOcorrencia, CancellationToken ct = default);
    Task<OcorrenciaItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OcorrenciaItem> CreateAsync(OcorrenciaItem item, CancellationToken ct = default);
    Task UpdateAsync(OcorrenciaItem item, CancellationToken ct = default);
    Task DeleteAsync(int id, int codigoOcorrencia, CancellationToken ct = default);
}
