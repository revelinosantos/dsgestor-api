using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public class DepartamentoRepository : IDepartamentoRepository
{
    private readonly DsGestorDbContext _context;

    public DepartamentoRepository(DsGestorDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Departamento>> GetAllAsync()
    {
        return await _context.Departamentos.AsNoTracking().ToListAsync();
    }

    public async Task<Departamento?> GetByIdAsync(int id)
    {
        return await _context.Departamentos.FindAsync(id);
    }

}
