using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface IMotivoOcorrenciaRepository
{
    Task<IEnumerable<MotivoOcorrencia>> GetAtivosAsync(CancellationToken ct = default);
    Task<MotivoOcorrencia?> GetByIdAsync(int id, CancellationToken ct = default);
}
