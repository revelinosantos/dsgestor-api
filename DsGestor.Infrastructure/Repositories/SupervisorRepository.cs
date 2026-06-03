using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public class SupervisorRepository : ISupervisorRepository
{
    private readonly DsGestorDbContext _context;
    
    public SupervisorRepository(DsGestorDbContext context)
    {
        _context = context;
    }
  
    public async Task<IEnumerable<Supervisor>> GetAllAsync()
    {
       return await _context.Supervisores.AsNoTracking().ToListAsync();
      }

    public async Task<Supervisor?> GetByIdAsync(int id)
    {
        return await _context.Supervisores.FindAsync(id);
    }

}
