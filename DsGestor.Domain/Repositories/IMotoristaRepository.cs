using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface IMotoristaRepository
{
    Task<IEnumerable<Motorista>> GetAllAsync();
    Task<Motorista?> GetByIdAsync(int matricula);
}