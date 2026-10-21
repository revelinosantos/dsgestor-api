using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public sealed class OcorrenciaItemRepository : IOcorrenciaItemRepository
{
    private readonly DsGestorDbContext _context;

    public OcorrenciaItemRepository(DsGestorDbContext context) => _context = context;

    public async Task<IEnumerable<OcorrenciaItem>> GetByOcorrenciaAsync(int codigoOcorrencia,
        CancellationToken ct = default) =>
        await _context.OcorrenciasItens.AsNoTracking()
            .Include(x => x.Produto)
            .Include(x => x.Motivo)
            .Where(x => x.CodigoOcorrencia == codigoOcorrencia)
            .OrderBy(x => x.NumeroItemNota)
            .ToListAsync(ct);

    public Task<OcorrenciaItem?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.OcorrenciasItens.AsNoTracking()
            .Include(x => x.Produto)
            .Include(x => x.Motivo)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<OcorrenciaItem> CreateAsync(OcorrenciaItem item, CancellationToken ct = default)
    {
        var ocorrencia = await ObterRascunhoAsync(item.CodigoOcorrencia, ct);

        if (item.NumeroTransacaoVenda != ocorrencia.NumeroTransacaoVenda)
            throw new InvalidOperationException("O item não pertence à transação da ocorrência.");

        ValidarDadosBasicos(item);

        var itemNota = await _context.NotasFiscaisOcorrencia.AsNoTracking()
            .FirstOrDefaultAsync(x => x.NumeroTransacaoVenda == item.NumeroTransacaoVenda &&
                                      x.NumeroTransacaoItem == item.NumeroItemNota, ct)
            ?? throw new InvalidOperationException("Item não encontrado na nota fiscal.");

        if (itemNota.CodigoProduto != item.CodigoProduto)
            throw new InvalidOperationException("O produto informado não corresponde ao item da nota fiscal.");
        if (item.QuantidadeOcorrencia > itemNota.Quantidade)
            throw new InvalidOperationException("Quantidade da ocorrência não pode superar a quantidade faturada.");

        var existe = await _context.OcorrenciasItens.CountAsync(x =>
            x.NumeroTransacaoVenda == item.NumeroTransacaoVenda &&
            x.NumeroItemNota == item.NumeroItemNota, ct) > 0;

        if (existe)
            throw new InvalidOperationException("Este item da NF já possui uma ocorrência.");

        item.Id = 0;
        item.QuantidadeFaturada = itemNota.Quantidade;
        item.ValorUnitario = itemNota.PrecoUnitario;
        item.ValorTotal = Math.Round(item.QuantidadeOcorrencia * item.ValorUnitario, 2);

        _context.OcorrenciasItens.Add(item);
        await _context.SaveChangesAsync(ct);
        return item;
    }

    public async Task UpdateAsync(OcorrenciaItem item, CancellationToken ct = default)
    {
        var atual = await _context.OcorrenciasItens.Include(x => x.Ocorrencia)
            .FirstOrDefaultAsync(x => x.Id == item.Id &&
                                      x.CodigoOcorrencia == item.CodigoOcorrencia, ct)
            ?? throw new KeyNotFoundException("Item não encontrado.");

        if (atual.Ocorrencia?.Status != StatusOcorrencia.RASCUNHO)
            throw new InvalidOperationException("Itens só podem ser alterados em RASCUNHO.");

        item.QuantidadeFaturada = atual.QuantidadeFaturada;
        Validar(item);

        atual.QuantidadeOcorrencia = item.QuantidadeOcorrencia;
        atual.CodigoMotivo = item.CodigoMotivo;
        atual.Observacao = item.Observacao;
        atual.ValorTotal = Math.Round(item.QuantidadeOcorrencia * atual.ValorUnitario, 2);

        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, int codigoOcorrencia, CancellationToken ct = default)
    {
        var atual = await _context.OcorrenciasItens.Include(x => x.Ocorrencia)
            .FirstOrDefaultAsync(x => x.Id == id && x.CodigoOcorrencia == codigoOcorrencia, ct)
            ?? throw new KeyNotFoundException("Item não encontrado.");

        if (atual.Ocorrencia?.Status != StatusOcorrencia.RASCUNHO)
            throw new InvalidOperationException("Itens só podem ser removidos em RASCUNHO.");

        _context.OcorrenciasItens.Remove(atual);
        await _context.SaveChangesAsync(ct);
    }

    private async Task<Ocorrencia> ObterRascunhoAsync(int id, CancellationToken ct)
    {
        var ocorrencia = await _context.Ocorrencias.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Ocorrência não encontrada.");

        if (ocorrencia.Status != StatusOcorrencia.RASCUNHO)
            throw new InvalidOperationException("Itens só podem ser incluídos em RASCUNHO.");

        return ocorrencia;
    }

    private static void Validar(OcorrenciaItem item)
    {
        ValidarDadosBasicos(item);
        if (item.QuantidadeOcorrencia <= 0)
            throw new InvalidOperationException("Quantidade deve ser maior que zero.");
        if (item.QuantidadeOcorrencia > item.QuantidadeFaturada)
            throw new InvalidOperationException("Quantidade da ocorrência não pode superar a faturada.");
    }

    private static void ValidarDadosBasicos(OcorrenciaItem item)
    {
        if (item.QuantidadeOcorrencia <= 0)
            throw new InvalidOperationException("Quantidade deve ser maior que zero.");
        if (item.CodigoMotivo <= 0)
            throw new InvalidOperationException("Informe o motivo.");
    }
}
