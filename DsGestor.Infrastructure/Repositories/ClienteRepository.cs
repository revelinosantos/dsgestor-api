using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly DsGestorDbContext _context;

    public ClienteRepository(DsGestorDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _context.Clientes
           .OrderBy( c => c.RazaoSocial)
           .AsNoTracking().ToListAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<IEnumerable<Cliente>>GetByVendedorAsync(int codigoVendedor)
    {
        return await _context.Clientes
           .Where(c => c.CodigoVendedor == codigoVendedor)
           .OrderBy( c => c.RazaoSocial)
           .AsNoTracking()
           .ToListAsync();
    }
}
