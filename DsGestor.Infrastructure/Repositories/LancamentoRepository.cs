using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public class LancamentoRepository : ILancamentoRepository
{
    private readonly DsGestorDbContext _context;
    private readonly IPromocaoRepository _promocaoRepository;
    private readonly IPromocaoItemRepository _promocaoItemRepository;
    private readonly IPromocaoClienteRepository _promocaoClienteRepository;
    private readonly IPromocaoOrigemPedidoRepository _promocaoOrigemPedidoRepository;

    public LancamentoRepository(
        DsGestorDbContext context,
        IPromocaoRepository promocaoRepository,
        IPromocaoItemRepository promocaoItemRepository,
        IPromocaoClienteRepository promocaoClienteRepository,
        IPromocaoOrigemPedidoRepository promocaoOrigemPedidoRepository)
    {
        _context = context;
        _promocaoRepository = promocaoRepository;
        _promocaoItemRepository = promocaoItemRepository;
        _promocaoClienteRepository = promocaoClienteRepository;
        _promocaoOrigemPedidoRepository = promocaoOrigemPedidoRepository;
    }

    public async Task<IEnumerable<Lancamento>> GetAllAsync()
        => await _context.Lancamentos
            .Include(c => c.Cliente)
            .Include(v => v.Vendedor!).ThenInclude(s => s.Supervisor)
            .Include(u => u.Usuario)
            .Include(a => a.UsuarioAut)
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .ToListAsync();

    public async Task<Lancamento?> GetByIdAsync(int id, bool incluirItens = false)
    {
        var query = _context.Lancamentos.AsQueryable();

        query = query.Include(c => c.Cliente);
        query = query.Include(v => v.Vendedor!).ThenInclude(s => s.Supervisor);
        query = query.Include(u => u.Usuario);
        query = query.Include(a => a.UsuarioAut);

        if (incluirItens)
        {
            query = query.Include(x => x.Itens)
                         .ThenInclude(d => d.Produto);
        }

        return await query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Lancamento>> GetByUsuarioAsync(int codigoUsuario)
        => await _context.Lancamentos
            .AsNoTracking()
            .Include(c => c.Cliente)
            .Include(v => v.Vendedor!).ThenInclude(s => s.Supervisor)
            .Include(u => u.Usuario)
            .Include(a => a.UsuarioAut)
            .Where(x => x.CodigoUsuario == codigoUsuario)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

    public async Task<IEnumerable<Lancamento>> GetByStatusAsync(StatusLancamento status)
        => await _context.Lancamentos
            .AsNoTracking()
            .Include(c => c.Cliente)
            .Include(v => v.Vendedor!).ThenInclude(s => s.Supervisor)
            .Include(u => u.Usuario)
            .Include(a => a.UsuarioAut)
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

    public async Task<StatusLancamento?> GetStatusAsync(int id)
        => await _context.Lancamentos
            .AsNoTracking()
            .Include(c => c.Cliente)
            .Include(v => v.Vendedor!).ThenInclude(s => s.Supervisor)
            .Include(u => u.Usuario)
            .Include(a => a.UsuarioAut)
            .Where(x => x.Id == id)
            .Select(x => (StatusLancamento?)x.Status)
            .FirstOrDefaultAsync();

    public async Task<Lancamento?> GetDraftAsync(
        string codigoFilial,
        int numRegiao,
        int codigoCliente,
        int codigoVendedor,
        int codigoUsuario,
        bool incluirItens = true)
    {
        var q = _context.Lancamentos.AsQueryable();

        q = q.Include(c => c.Cliente);
        q = q.Include(v => v.Vendedor!).ThenInclude(s => s.Supervisor);
        q = q.Include(u => u.Usuario);
        q = q.Include(a => a.UsuarioAut);

        if (incluirItens)
        {
            q = q.Include(x => x.Itens)
                 .ThenInclude(d => d.Produto);
        }

        return await q.AsNoTracking()
            .Where(x =>
                x.Status == StatusLancamento.RASCUNHO &&
                x.CodigoFilial == codigoFilial &&
                x.NumRegiao == numRegiao &&
                x.CodigoCliente == codigoCliente &&
                x.CodigoVendedor == codigoVendedor &&
                x.CodigoUsuario == codigoUsuario
            )
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<Lancamento> CreateOrGetDraftAsync(Lancamento draft)
    {
        draft.Status = StatusLancamento.RASCUNHO;

        var existing = await GetDraftAsync(
            draft.CodigoFilial,
            draft.NumRegiao,
            draft.CodigoCliente,
            draft.CodigoVendedor,
            draft.CodigoUsuario,
            incluirItens: true);

        if (existing is not null)
            return existing;

        _context.Lancamentos.Add(draft);
        await _context.SaveChangesAsync();
        return draft;
    }

    public async Task<bool> FinalizarAsync(int id)
    {
        var entity = await _context.Lancamentos.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return false;

        if (entity.Status != StatusLancamento.RASCUNHO)
            return false;

        entity.Status = StatusLancamento.PENDENTE;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Lancamento> CreateAsync(Lancamento entity)
    {
        _context.Lancamentos.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Lancamento entity)
    {
        var current = await _context.Lancamentos
            .FirstOrDefaultAsync(x => x.Id == entity.Id);

        if (current is null) return;

        if (current.Status != StatusLancamento.RASCUNHO &&
            current.Status != StatusLancamento.PENDENTE)
        {
            throw new InvalidOperationException("Lançamento não pode ser alterado neste status.");
        }

        current.Data = entity.Data;
        current.CodigoFilial = entity.CodigoFilial;
        current.NumRegiao = entity.NumRegiao;
        current.CodigoVendedor = entity.CodigoVendedor;
        current.CodigoCliente = entity.CodigoCliente;
        current.Observacao = entity.Observacao;

        await _context.SaveChangesAsync();
    }
    public async Task UpdateStatusAsync(int id, StatusLancamento status, int? codigoUsuarioAut)
    {
        var entity = await _context.Lancamentos.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return;

        if (entity.Status == StatusLancamento.AUTORIZADO || entity.Status == StatusLancamento.REJEITADO)
            throw new InvalidOperationException("Lançamento já finalizado (autorizado/rejeitado) e não pode ter status alterado.");

        if (entity.Status == StatusLancamento.RASCUNHO &&
            (status == StatusLancamento.AUTORIZADO || status == StatusLancamento.REJEITADO))
        {
            throw new InvalidOperationException("Finalize o rascunho antes de autorizar/rejeitar.");
        }

        entity.Status = status;

        if (status == StatusLancamento.AUTORIZADO || status == StatusLancamento.REJEITADO)
        {
            entity.CodigoUsuarioAut = codigoUsuarioAut;
            entity.DataAutorizacao = DateTime.Now;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Lancamentos
            .Include(x => x.Itens)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null) return;

        if (entity.Status == StatusLancamento.AUTORIZADO || entity.Status == StatusLancamento.REJEITADO)
            throw new InvalidOperationException("Lançamento autorizado/rejeitado não pode ser excluído.");

        if (entity.Itens is not null && entity.Itens.Count > 0)
            _context.LancamentosDet.RemoveRange(entity.Itens);

        _context.Lancamentos.Remove(entity);

        await _context.SaveChangesAsync();
    }

    public async Task<int> AutorizarAsync(int id, int codigoUsuarioAut, CancellationToken ct)
    {
        var lanc = await _context.Lancamentos
            .Include(x => x.Itens)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (lanc == null)
            throw new InvalidOperationException("Lançamento não encontrado.");

        if ((int)lanc.Status != (int)StatusLancamento.PENDENTE)
            throw new InvalidOperationException("Somente lançamentos pendentes podem ser autorizados.");

        if (lanc.Itens == null || lanc.Itens.Count == 0)
            throw new InvalidOperationException("Lançamento sem itens.");

        // idempotência
        if (lanc.CodigoPromocao.HasValue && lanc.CodigoPromocao.Value > 0)
            return lanc.CodigoPromocao.Value;

        await using var trx = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            var dataAut = DateTime.Now;
            var dataAutDia = dataAut.Date;

            var codigoPromocao = await _promocaoRepository.GetNextIdAsync(ct);

            var promocao = new Promocao
            {
                Id = codigoPromocao,
                IdentificadorPromocao = "PRMCON",
                TipoPolitica = "D",
                DescricaoResumida = $"PROMO CLIENTE {lanc.CodigoCliente}",
                DescricaoDetalhada = $"PROMO CLIENTE {lanc.CodigoCliente}",
                DataInicial = dataAutDia,
                DataFinal = dataAutDia,
                TipoPromocao = "C",
                TipoRestricao = 1,
                CodFuncLanc = codigoUsuarioAut,
                DataLanc = dataAut,
                AlteraDesconto = "S",
                SyncFv = "N",
                CodReferencia = $"LANC:{lanc.Id}"
            };

            await _promocaoRepository.AddAsync(promocao, ct);
            await _context.SaveChangesAsync(ct);

            var promocaoCliente  = new PromocaoCliente
            {
                  Id = codigoPromocao,
                  CodigoCliente = lanc.CodigoCliente,
            };

            await _promocaoClienteRepository.AddAsync(promocaoCliente, ct);
            await _context.SaveChangesAsync(ct);


            foreach (var origem in new[] { "T",  "R", "F" })
            {
                var promocaoOrigemPedido = new PromocaoOrigemPedido
                {
                    Id = codigoPromocao,
                    OrigemPed = origem,
                };
                await _promocaoOrigemPedidoRepository.AddAsync(promocaoOrigemPedido, ct);
            };

            await _context.SaveChangesAsync(ct);

            foreach (var itemLanc in lanc.Itens)
            {
                var percDesc = itemLanc.PercDescontoAut;

                foreach (var origem in new[] { "T",     "R", "F" })
                {
                    var codigoPromocaoItem = await _promocaoItemRepository.GetNextIdAsync(ct);

                    var item = new PromocaoItem
                    {
                        Id = codigoPromocaoItem,
                        CodigoCliente = lanc.CodigoCliente,
                        CodigoProduto = itemLanc.CodigoProduto,
                        PercDesc = percDesc,

                        DtInicio = dataAutDia,
                        DtFim = dataAutDia,

                        CodFuncLanc = codigoUsuarioAut,
                        DataLanc = dataAut,
                        CodFuncUltAlter = codigoUsuarioAut,
                        DataUltAlter = dataAut,

                        CodigoPromocao = promocao.Id,
                        TipoPoliticaPromocaoMed = "D",

                        OrigemPed = origem, 

                        Descricao = $"PROMO CLIENTE {lanc.CodigoCliente}",
                        GrupoValidacao = "P",
                        SyncFv = "N",
                        IonSync = "N"
                    };

                    await _promocaoItemRepository.AddAsync(item, ct);
                }
            }

            lanc.Status = StatusLancamento.AUTORIZADO;
            lanc.CodigoPromocao = promocao.Id;
            lanc.CodigoUsuarioAut = codigoUsuarioAut;
            lanc.DataAutorizacao = dataAut;

            await _context.SaveChangesAsync(ct);
            await trx.CommitAsync(ct);

            return promocao.Id;
        }
        catch
        {
            await trx.RollbackAsync(ct);
            throw;
        }
    }

}