using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public class MotoristaRepository : IMotoristaRepository
{
    private readonly DsGestorDbContext _context;

    public MotoristaRepository(DsGestorDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Motorista>> GetAllAsync()
    {
        return await _context.Motoristas
            .AsNoTracking()
            .Where(x => x.Situacao == "A")
            .OrderBy(x => x.Nome)
            .ToListAsync();
    }

    public async Task<Motorista?> GetByIdAsync(int matricula)
    {
        return await _context.Motoristas
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Matricula == matricula);
    }
}