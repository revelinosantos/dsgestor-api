using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public sealed class MotivoOcorrenciaRepository : IMotivoOcorrenciaRepository
{
    private readonly DsGestorDbContext _context;

    public MotivoOcorrenciaRepository(DsGestorDbContext context) => _context = context;

    public async Task<IEnumerable<MotivoOcorrencia>> GetAtivosAsync(CancellationToken ct = default) =>
        await _context.MotivosOcorrencia.AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToListAsync(ct);

    public Task<MotivoOcorrencia?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.MotivosOcorrencia.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
}
