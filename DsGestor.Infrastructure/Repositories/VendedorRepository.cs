using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public class VendedorRepository: IVendedorRepository
{
    private readonly DsGestorDbContext _context;
    public VendedorRepository(DsGestorDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Vendedor>> GetAllAsync()
    {
        return await _context.Vendedores
               .Include( u => u.Supervisor)
               .Where( v => v.DataTermino == null && !v.Nome.Contains("(OL)"))
               .OrderBy( v => v.Nome)
               .AsNoTracking().ToListAsync();
    }

    public async Task<Vendedor?> GetByIdAsync(int id)
    {
        return await _context.Vendedores
               .Include( v => v.Supervisor)
               .Where( v => v.DataTermino == null && !v.Nome.Contains("(OL)"))
               .FirstOrDefaultAsync( v => v.Id == id);
    }


}
