
using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface IPromocaoItemRepository
{
    Task<int> GetNextIdAsync(CancellationToken ct);
    Task AddAsync(PromocaoItem item, CancellationToken ct);
    Task<IReadOnlyList<PromocaoItem>> GetByPromocaoIdAsync(int codigoPromocao, CancellationToken ct);
    Task<PromocaoItem?> GetByIdAsync(long id, CancellationToken ct);
}