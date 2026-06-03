using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface IPromocaoRepository
{
    Task<int> GetNextIdAsync(CancellationToken ct);
    Task AddAsync(Promocao promocao, CancellationToken ct);
    Task<Promocao?> GetByIdAsync(int id, CancellationToken ct);
    
}