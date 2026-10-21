using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public sealed class OcorrenciaRepository : IOcorrenciaRepository
{
    private readonly DsGestorDbContext _context;

    public OcorrenciaRepository(DsGestorDbContext context) => _context = context;

    public async Task<IEnumerable<Ocorrencia>> GetAllAsync(CancellationToken ct = default) =>
        await BaseQuery(false).OrderByDescending(x => x.Data).ThenByDescending(x => x.Id).ToListAsync(ct);

    public Task<Ocorrencia?> GetByIdAsync(int id, bool incluirItens = false, CancellationToken ct = default) =>
        BaseQuery(incluirItens).FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Ocorrencia?> GetDraftAsync(string codigoFilial, decimal numeroNota,
        string serieNota, int codigoUsuario, bool incluirItens = true,
        CancellationToken ct = default) =>
        BaseQuery(incluirItens).FirstOrDefaultAsync(x =>
            x.CodigoFilial == codigoFilial && x.NumeroNota == numeroNota &&
            x.SerieNota == serieNota && x.CodigoUsuario == codigoUsuario &&
            x.Status == StatusOcorrencia.RASCUNHO, ct);

    public async Task<Ocorrencia> CreateOrGetDraftAsync(Ocorrencia ocorrencia, CancellationToken ct = default)
    {
        var existente = await GetDraftAsync(ocorrencia.CodigoFilial, ocorrencia.NumeroNota,
            ocorrencia.SerieNota, ocorrencia.CodigoUsuario, true, ct);

        if (existente is not null)
            return existente;

        ocorrencia.Id = 0;
        ocorrencia.Data = DateTime.Now;
        ocorrencia.Status = StatusOcorrencia.RASCUNHO;
        ocorrencia.Itens.Clear();
        ocorrencia.Historicos.Clear();

        _context.Ocorrencias.Add(ocorrencia);
        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(ocorrencia.Id, true, ct) ?? ocorrencia;
    }

    public async Task<Ocorrencia> CreateWithItemsAsync(
        Ocorrencia ocorrencia, CancellationToken ct = default)
    {
        if (ocorrencia.Itens.Count == 0)
            throw new InvalidOperationException("Selecione pelo menos um item para a ocorrência.");

        if (ocorrencia.Itens.GroupBy(x => x.NumeroItemNota).Any(x => x.Count() > 1))
            throw new InvalidOperationException("Há itens repetidos na ocorrência.");

        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            foreach (var item in ocorrencia.Itens)
            {
                if (item.NumeroTransacaoVenda != ocorrencia.NumeroTransacaoVenda)
                    throw new InvalidOperationException("Um item não pertence à transação da ocorrência.");
                if (item.QuantidadeOcorrencia <= 0)
                    throw new InvalidOperationException("Quantidade da ocorrência deve ser maior que zero.");
                if (item.CodigoMotivo <= 0)
                    throw new InvalidOperationException("Informe o motivo de todos os itens.");

                var itemNota = await _context.NotasFiscaisOcorrencia.AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.NumeroTransacaoVenda == item.NumeroTransacaoVenda &&
                        x.NumeroTransacaoItem == item.NumeroItemNota, ct)
                    ?? throw new InvalidOperationException(
                        $"Item {item.NumeroItemNota} não encontrado na nota fiscal.");

                if (itemNota.CodigoProduto != item.CodigoProduto)
                    throw new InvalidOperationException(
                        $"O produto do item {item.NumeroItemNota} não corresponde à nota fiscal.");
                if (item.QuantidadeOcorrencia > itemNota.Quantidade)
                    throw new InvalidOperationException(
                        $"A quantidade do item {item.NumeroItemNota} supera a quantidade faturada.");

                // CountAsync evita o literal SQL FALSE, incompatível com versões antigas do Oracle.
                var jaUtilizado = await _context.OcorrenciasItens.AsNoTracking().CountAsync(x =>
                    x.NumeroTransacaoVenda == item.NumeroTransacaoVenda &&
                    x.NumeroItemNota == item.NumeroItemNota, ct) > 0;

                if (jaUtilizado)
                    throw new InvalidOperationException(
                        $"O item {item.NumeroItemNota} da NF já possui uma ocorrência.");

                item.Id = 0;
                item.CodigoOcorrencia = 0;
                item.QuantidadeFaturada = itemNota.Quantidade;
                item.ValorUnitario = itemNota.PrecoUnitario;
                item.ValorTotal = Math.Round(item.QuantidadeOcorrencia * itemNota.PrecoUnitario, 2);
            }

            ocorrencia.Id = 0;
            ocorrencia.Data = DateTime.Now;
            ocorrencia.Status = StatusOcorrencia.RASCUNHO;
            ocorrencia.Historicos.Clear();

            _context.Ocorrencias.Add(ocorrencia);
            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return ocorrencia;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task UpdateAsync(Ocorrencia ocorrencia, CancellationToken ct = default)
    {
        var atual = await _context.Ocorrencias.FirstOrDefaultAsync(x => x.Id == ocorrencia.Id, ct)
            ?? throw new KeyNotFoundException("Ocorrência não encontrada.");

        if (atual.Status != StatusOcorrencia.RASCUNHO)
            throw new InvalidOperationException("Somente ocorrência em RASCUNHO pode ser alterada.");

        atual.CodigoMotorista = ocorrencia.CodigoMotorista;
        atual.DevolucaoTotal = ocorrencia.DevolucaoTotal;
        atual.Observacao = ocorrencia.Observacao;

        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> FinalizarRascunhoAsync(int id, int codigoUsuario, CancellationToken ct = default)
    {
        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        var atual = await _context.Ocorrencias.Include(x => x.Itens)
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Ocorrência não encontrada.");

        if (atual.Status != StatusOcorrencia.RASCUNHO)
            throw new InvalidOperationException("Somente rascunho pode ser aberto.");
        if (atual.Itens.Count == 0)
            throw new InvalidOperationException("Inclua pelo menos um item na ocorrência.");

        var anterior = atual.Status;
        atual.Status = StatusOcorrencia.ABERTA;
        AddHistorico(atual.Id, codigoUsuario, anterior, atual.Status, "Ocorrência aberta.");

        await _context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return true;
    }

    public async Task UpdateStatusAsync(int id, StatusOcorrencia status, int codigoUsuario,
        string? observacao = null, CancellationToken ct = default)
    {
        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        var atual = await _context.Ocorrencias.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Ocorrência não encontrada.");

        ValidarTransicao(atual.Status, status);
        var anterior = atual.Status;
        atual.Status = status;
        var agora = DateTime.Now;

        switch (status)
        {
            case StatusOcorrencia.ENTREGUE_MOTORISTA:
                atual.DataEntregaMotorista = agora;
                atual.CodigoUsuarioEntrega = codigoUsuario;
                break;
            case StatusOcorrencia.RETORNADA:
                atual.DataRetorno = agora;
                atual.CodigoUsuarioRetorno = codigoUsuario;
                break;
            case StatusOcorrencia.FINALIZADA:
                atual.DataFinalizacao = agora;
                atual.CodigoUsuarioFinalizacao = codigoUsuario;
                break;
        }

        AddHistorico(id, codigoUsuario, anterior, status, observacao);
        await _context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    public async Task ConfirmarCreditoAsync(int id, decimal? numeroCredito, decimal? numeroNotaCredito,
        int codigoUsuario, string? observacao = null, CancellationToken ct = default)
    {
        if (!numeroCredito.HasValue && !numeroNotaCredito.HasValue)
            throw new InvalidOperationException("Informe a transação ou a nota de crédito.");

        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        var atual = await _context.Ocorrencias.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Ocorrência não encontrada.");

        if (atual.Status != StatusOcorrencia.RETORNADA)
            throw new InvalidOperationException("Somente ocorrência RETORNADA pode receber crédito.");

        var anterior = atual.Status;
        atual.NumeroCredito = numeroCredito;
        atual.NumeroNotaCredito = numeroNotaCredito;
        atual.DataCredito = DateTime.Now;
        atual.CodigoUsuarioCredito = codigoUsuario;
        atual.Status = StatusOcorrencia.CREDITO_CONFERIDO;

        AddHistorico(id, codigoUsuario, anterior, atual.Status, observacao);
        await _context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    public async Task CancelarAsync(int id, int codigoUsuario, string motivo, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new InvalidOperationException("Informe o motivo do cancelamento.");

        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        var atual = await _context.Ocorrencias.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Ocorrência não encontrada.");

        if (atual.Status is StatusOcorrencia.FINALIZADA or StatusOcorrencia.CANCELADA)
            throw new InvalidOperationException("Ocorrência finalizada ou cancelada não pode ser cancelada.");

        var anterior = atual.Status;
        atual.Status = StatusOcorrencia.CANCELADA;
        atual.DataCancelamento = DateTime.Now;
        atual.CodigoUsuarioCancelamento = codigoUsuario;
        atual.MotivoCancelamento = motivo.Trim();

        AddHistorico(id, codigoUsuario, anterior, atual.Status, motivo.Trim());
        await _context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var atual = await _context.Ocorrencias.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Ocorrência não encontrada.");

        if (atual.Status != StatusOcorrencia.RASCUNHO)
            throw new InvalidOperationException("Somente rascunho pode ser excluído.");

        _context.Ocorrencias.Remove(atual);
        await _context.SaveChangesAsync(ct);
    }

    private IQueryable<Ocorrencia> BaseQuery(bool incluirItens)
    {
        IQueryable<Ocorrencia> query = _context.Ocorrencias.AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Vendedor)
            .Include(x => x.MotoristaNota)
            .Include(x => x.Motorista)
            .Include(x => x.Usuario);

        if (incluirItens)
        {
            query = query
                .Include(x => x.Itens).ThenInclude(x => x.Produto)
                .Include(x => x.Itens).ThenInclude(x => x.Motivo)
                .Include(x => x.Historicos).ThenInclude(x => x.Usuario);
        }

        return query.AsSplitQuery();
    }

    private void AddHistorico(int id, int usuario, StatusOcorrencia? anterior,
        StatusOcorrencia novo, string? observacao) =>
        _context.OcorrenciasHistorico.Add(new OcorrenciaHistorico
        {
            CodigoOcorrencia = id,
            Data = DateTime.Now,
            CodigoUsuario = usuario,
            StatusAnterior = anterior,
            StatusNovo = novo,
            Observacao = observacao
        });

    private static void ValidarTransicao(StatusOcorrencia atual, StatusOcorrencia novo)
    {
        var valida = (atual, novo) switch
        {
            (StatusOcorrencia.ABERTA, StatusOcorrencia.ENTREGUE_MOTORISTA) => true,
            (StatusOcorrencia.ENTREGUE_MOTORISTA, StatusOcorrencia.RETORNADA) => true,
            (StatusOcorrencia.CREDITO_CONFERIDO, StatusOcorrencia.FINALIZADA) => true,
            _ => false
        };

        if (!valida)
            throw new InvalidOperationException($"Transição inválida: {atual} → {novo}.");
    }
}
