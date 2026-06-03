using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;
public interface ISupervisorRepository
{
    Task<IEnumerable<Supervisor>> GetAllAsync();
    Task<Supervisor?> GetByIdAsync(int id);
}
