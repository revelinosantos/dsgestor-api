using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly DsGestorDbContext _context;

    public UsuarioRepository(DsGestorDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Include( u => u.Vendedor)
            .Include( u => u.Supervisor)
            .ToListAsync();
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios
            .Include( u => u.Vendedor)
            .Include( u => u.Supervisor)
            .FirstOrDefaultAsync( u => u.Id == id);
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios
            .Include( u => u.Vendedor)
            .Include( u => u.Supervisor)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario?> GetByCodigoAsync(string codigo)
    {
        return await _context.Usuarios
            .Include( u => u.Vendedor)
            .Include( u => u.Supervisor)
            .FirstOrDefaultAsync(u => u.Codigo == codigo);
    }

    public async Task AddAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Usuarios.FindAsync(id);
        if (entity != null)
        {
            _context.Usuarios.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
