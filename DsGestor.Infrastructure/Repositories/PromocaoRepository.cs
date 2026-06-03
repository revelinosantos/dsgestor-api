using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public sealed class PromocaoRepository : IPromocaoRepository
{
    private readonly DsGestorDbContext _context;

    public PromocaoRepository(DsGestorDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Promocao promocao, CancellationToken ct)
    {
        await _context.Promocoes.AddAsync(promocao, ct);
    }

    public async Task<Promocao?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Promocoes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<int> GetNextIdAsync(CancellationToken ct)
    {
        var result = await _context.Database
            .SqlQueryRaw<int>(@"SELECT ECOACRE.DFSEQ_PCPROMOCAOMED.NEXTVAL AS Value FROM DUAL")
            .ToListAsync(ct);

        return result.First();
     }
}