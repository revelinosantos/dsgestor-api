using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public sealed class NotaFiscalOcorrenciaRepository : INotaFiscalOcorrenciaRepository
{
    private readonly DsGestorDbContext _context;

    public NotaFiscalOcorrenciaRepository(DsGestorDbContext context) => _context = context;

    public async Task<IReadOnlyList<NotaFiscalOcorrenciaView>> PesquisarAsync(
        decimal numeroNota, string codigoFilial, string serie, CancellationToken ct = default) =>
        await _context.NotasFiscaisOcorrencia.AsNoTracking()
            .Where(x => x.NumeroNota == numeroNota &&
                        x.CodigoFilial == codigoFilial &&
                        x.Serie == serie)
            .OrderBy(x => x.NumeroTransacaoItem)
            .ToListAsync(ct);

    public async Task<bool> ItemPossuiOcorrenciaAsync(decimal numeroTransacaoVenda,
        int numeroTransacaoItem, CancellationToken ct = default) =>
        await _context.OcorrenciasItens.AsNoTracking().CountAsync(x =>
            x.NumeroTransacaoVenda == numeroTransacaoVenda &&
            x.NumeroItemNota == numeroTransacaoItem, ct) > 0;
}
