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
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    private static void ValidarPrecoMinimo(decimal? precoUnitario, decimal? precoMinimo, string campo)
    {
        var minimo = precoMinimo ?? 0m;
        var unit = precoUnitario ?? 0m;

        if (minimo > 0m && unit < minimo)
        {
            throw new InvalidOperationException(
                $"{campo} não pode ser inferior ao preço mínimo ({minimo:N2})."
            );
        }
    }

    public async Task<LancamentoDet> CreateAsync(LancamentoDet det)
    {
        ValidarPrecoMinimo(det.PrecoUnitario, det.PrecoMinimo, "Preço unitário");

        det.QuantidadeAut = det.Quantidade;
        det.PrecoUnitarioAut = det.PrecoUnitario;
        det.PercDescontoAut = det.PercDesconto;
        det.MargemAut = det.Margem;
        det.PrecoMinimo = det.PrecoMinimo;

        _context.LancamentosDet.Add(det);
        await _context.SaveChangesAsync();
        return det;
    }

    public async Task UpdateAsync(LancamentoDet det)
    {
        var atual = await _context.LancamentosDet
            .Include(x => x.Lancamento)
            .FirstOrDefaultAsync(x => x.Id == det.Id);

        if (atual is null)
            throw new Exception("Item não encontrado.");

        ValidarPrecoMinimo(det.PrecoUnitario, det.PrecoMinimo, "Preço unitário");

        atual.CodigoProduto = det.CodigoProduto;
        atual.CodigoIcmTab = det.CodigoIcmTab;
        atual.PrecoCustoFin = det.PrecoCustoFin;
        atual.PrecoVenda = det.PrecoVenda;
        atual.MargemIdeal = det.MargemIdeal;

        atual.Quantidade = det.Quantidade;
        atual.PrecoUnitario = det.PrecoUnitario;
        atual.PercDesconto = det.PercDesconto;
        atual.Margem = det.Margem;
        atual.PrecoMinimo = det.PrecoMinimo;

        // enquanto NÃO autorizado, AUT acompanha o normal
        if (atual.Lancamento != null && atual.Lancamento.Status != StatusLancamento.AUTORIZADO)
        {
            atual.QuantidadeAut = atual.Quantidade;
            atual.PrecoUnitarioAut = atual.PrecoUnitario;
            atual.PercDescontoAut = atual.PercDesconto;
            atual.MargemAut = atual.Margem;
            atual.PrecoMinimo = det.PrecoMinimo;
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAutorizacaoAsync(LancamentoDet det, int codigoUsuarioAut)
    {
        var atual = await _context.LancamentosDet
            .Include(x => x.Lancamento)
            .FirstOrDefaultAsync(x => x.Id == det.Id);

        if (atual is null)
            throw new Exception("Item não encontrado.");

        if (atual.Lancamento is null)
            throw new Exception("Lançamento do item não encontrado.");

        ValidarPrecoMinimo(det.PrecoUnitarioAut, atual.PrecoMinimo, "Preço unitário autorizado");

        atual.QuantidadeAut = det.QuantidadeAut;
        atual.PrecoUnitarioAut = det.PrecoUnitarioAut;
        atual.PercDescontoAut = det.PercDescontoAut;
        atual.MargemAut = det.MargemAut;

        atual.Lancamento.CodigoUsuarioAut = codigoUsuarioAut;
        atual.Lancamento.DataAutorizacao = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var atual = await _context.LancamentosDet.FirstOrDefaultAsync(x => x.Id == id);
        if (atual is null) return;

        _context.LancamentosDet.Remove(atual);
        await _context.SaveChangesAsync();
    }
}