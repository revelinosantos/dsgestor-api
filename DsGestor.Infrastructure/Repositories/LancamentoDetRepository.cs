using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public class LancamentoDetRepository : ILancamentoDetRepository
{
    private readonly DsGestorDbContext _context;

    public LancamentoDetRepository(DsGestorDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LancamentoDet>> GetByLancamentoAsync(int idLanc)
    {
        return await _context.LancamentosDet
            .AsNoTracking()
            .Include(x => x.Produto)
            .Where(x => x.CodigoLancamento == idLanc)
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<LancamentoDet?> GetByIdAsync(int id)
    {
        return await _context.LancamentosDet
            .AsNoTracking()
            .Include(x => x.Produto)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<LancamentoDet> CreateAsync(LancamentoDet det)
    {
        ValidarPrecoMinimo(det.PrecoUnitario, det.PrecoMinimo, "Preço unitário");

        /*
         * Regra:
         * No rascunho, o valor solicitado e o valor autorizado nascem iguais.
         */
        det.QuantidadeAut = det.Quantidade;
        det.PrecoUnitarioAut = det.PrecoUnitario;
        det.PercDescontoAut = det.PercDesconto;
        det.MargemAut = det.Margem;

        _context.LancamentosDet.Add(det);
        await _context.SaveChangesAsync();

        return det;
    }

    public async Task UpdateAsync(LancamentoDet det)
    {
        var atual = await _context.LancamentosDet
            .Include(x => x.Lancamento)
            .FirstOrDefaultAsync(x =>
                x.Id == det.Id &&
                x.CodigoLancamento == det.CodigoLancamento);

        if (atual is null)
            throw new KeyNotFoundException("Item não encontrado.");

        if (atual.Lancamento is null)
            throw new InvalidOperationException("Lançamento do item não encontrado.");

        /*
         * Update normal só deve ser usado em RASCUNHO.
         * Em rascunho, original = AUT.
         */
        if (atual.Lancamento.Status != StatusLancamento.RASCUNHO)
        {
            throw new InvalidOperationException(
                "A atualização normal de item só é permitida em RASCUNHO."
            );
        }

        ValidarPrecoMinimo(det.PrecoUnitario, det.PrecoMinimo, "Preço unitário");

        atual.CodigoProduto = det.CodigoProduto;
        atual.CodigoIcmTab = det.CodigoIcmTab;
        atual.PrecoCustoFin = det.PrecoCustoFin;
        atual.PrecoVenda = det.PrecoVenda;
        atual.PrecoMinimo = det.PrecoMinimo;
        atual.MargemIdeal = det.MargemIdeal;

        atual.Quantidade = det.Quantidade;
        atual.PrecoUnitario = det.PrecoUnitario;
        atual.PercDesconto = det.PercDesconto;
        atual.Margem = det.Margem;

        /*
         * Enquanto rascunho:
         * os campos autorizados acompanham exatamente o solicitado.
         */
        atual.QuantidadeAut = det.Quantidade;
        atual.PrecoUnitarioAut = det.PrecoUnitario;
        atual.PercDescontoAut = det.PercDesconto;
        atual.MargemAut = det.Margem;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAutorizacaoAsync(LancamentoDet det, int codigoUsuarioAut)
    {
        var atual = await _context.LancamentosDet
            .Include(x => x.Lancamento)
            .FirstOrDefaultAsync(x =>
                x.Id == det.Id &&
                x.CodigoLancamento == det.CodigoLancamento);

        if (atual is null)
            throw new KeyNotFoundException("Item não encontrado.");

        if (atual.Lancamento is null)
            throw new InvalidOperationException("Lançamento do item não encontrado.");

        if (atual.Lancamento.Status != StatusLancamento.PENDENTE)
        {
            throw new InvalidOperationException(
                "A autorização de item só é permitida em lançamento PENDENTE."
            );
        }

        /*
         * Importante:
         * Em autorização, valida o preço AUT contra o preço mínimo já gravado no item.
         */
        ValidarPrecoMinimo(
            det.PrecoUnitarioAut,
            atual.PrecoMinimo,
            "Preço unitário autorizado"
        );

        /*
         * Regra:
         * Em PENDENTE, gerente altera SOMENTE os campos AUT.
         * Os campos originais do vendedor permanecem intactos para auditoria.
         */
        atual.QuantidadeAut = det.QuantidadeAut;
        atual.PrecoUnitarioAut = det.PrecoUnitarioAut;
        atual.PercDescontoAut = det.PercDescontoAut;
        atual.MargemAut = det.MargemAut;

        atual.Lancamento.CodigoUsuarioAut = codigoUsuarioAut;
        atual.Lancamento.DataAutorizacao = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, int idLanc)
    {
        var atual = await _context.LancamentosDet
            .Include(x => x.Lancamento)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CodigoLancamento == idLanc);

        if (atual is null)
            throw new KeyNotFoundException("Item não encontrado.");

        if (atual.Lancamento is null)
            throw new InvalidOperationException("Lançamento do item não encontrado.");

        if (atual.Lancamento.Status != StatusLancamento.RASCUNHO &&
            atual.Lancamento.Status != StatusLancamento.PENDENTE)
        {
            throw new InvalidOperationException(
                "Itens só podem ser removidos em lançamentos RASCUNHO ou PENDENTE."
            );
        }

        _context.LancamentosDet.Remove(atual);
        await _context.SaveChangesAsync();
    }

    private static void ValidarPrecoMinimo(
        decimal? precoUnitario,
        decimal? precoMinimo,
        string campo)
    {
        var minimo = precoMinimo ?? 0m;
        var unit = precoUnitario ?? 0m;

        /*
         * Preço mínimo zero significa "sem mínimo configurado".
         * Só bloqueia quando o mínimo é maior que zero
         * e o preço unitário autorizado/solicitado está abaixo dele.
         */
        if (minimo > 0m && unit < minimo)
        {
            throw new InvalidOperationException(
                $"{campo} não pode ser inferior ao preço mínimo ({minimo:N2})."
            );
        }
    }
}