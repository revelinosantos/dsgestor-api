using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface IVendedorRepository
{
    Task<IEnumerable<Vendedor>> GetAllAsync();
    Task<Vendedor?> GetByIdAsync(int id);
}
