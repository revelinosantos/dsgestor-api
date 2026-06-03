using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly DsGestorDbContext _context;

    public ProdutoRepository(DsGestorDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Produto>> GetAllAsync()
    {
        return await _context.Produtos
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
         return _context.Produtos
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);
     }

    public async Task<IEnumerable<Produto>> GetByFilialAsync(string codigoFilial)
    {
        return await _context.Produtos
            .AsNoTracking()
            .Where(p => p.CodigoFilial == codigoFilial)
            .OrderBy( p => p.Descricao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> GetByFilialAndRegiaoAsync(
        string codigoFilial,
        int codigoRegiao)
    {
        return await _context.Produtos
            .AsNoTracking()
            .Where(p =>
                p.CodigoFilial == codigoFilial &&
                p.CodigoRegiao == codigoRegiao
            )
            .OrderBy( p => p.Descricao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> GetByFilialAndRegiaoAndDescricaoAsync(
        string codigoFilial,
        int codigoRegiao,
        string descricao)
    {
        descricao = (descricao ?? "").Trim().ToUpper();

        return await _context.Produtos
            .AsNoTracking()
            .Where(p =>
                p.CodigoFilial == codigoFilial &&
                p.CodigoRegiao == codigoRegiao &&
                EF.Functions.Like(p.Descricao.ToUpper(), $"%{descricao}%")
            )
            .OrderBy( p => p.Descricao)
            .ToListAsync();
    }

    public async Task<Produto?> GetByFilialAndRegiaoByIdAsync(
        string codigoFilial,
        int codigoRegiao,
        int id)
    {
            return _context.Produtos
                .AsNoTracking()
                .FirstOrDefault(p =>
                      p.CodigoFilial == codigoFilial &&
                      p.CodigoRegiao == codigoRegiao &&
                 p.Id == id);
     
    }
}
