using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<IEnumerable<Cliente>> GetByVendedorAsync(int codigoVendedor);

}
