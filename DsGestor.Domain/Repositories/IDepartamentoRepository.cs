using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface IDepartamentoRepository
{
    Task<IEnumerable<Departamento>> GetAllAsync();
    Task<Departamento?> GetByIdAsync(int id);

}
