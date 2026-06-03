using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public sealed class PromocaoItemRepository : IPromocaoItemRepository
{
    private readonly DsGestorDbContext _context;

    public PromocaoItemRepository(DsGestorDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PromocaoItem item, CancellationToken ct)
    {
        await _context.PromocaoItens.AddAsync(item, ct);
    }

    public async Task<IReadOnlyList<PromocaoItem>> GetByPromocaoIdAsync(int codigoPromocao, CancellationToken ct)
    {
        return await _context.PromocaoItens
            .AsNoTracking()
            .Where(x => x.CodigoPromocao == codigoPromocao)
            .OrderBy(x => x.Id)
            .ToListAsync(ct);
    }

    public async Task<PromocaoItem?> GetByIdAsync(long id, CancellationToken ct)
    {
        return await _context.PromocaoItens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
    public async Task<int> GetNextIdAsync(CancellationToken ct)
    {
        var result = await _context.Database
            .SqlQueryRaw<int>(@"SELECT ECOACRE.DFSEQ_PCDESCONTO.NEXTVAL AS Value FROM DUAL")
            .ToListAsync(ct);

        return result.First();
     
    }
}