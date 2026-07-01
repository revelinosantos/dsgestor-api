using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public class CoteFacilProdutoRepository : ICoteFacilProdutoRepository
{
    private readonly DsGestorDbContext _context;

    public CoteFacilProdutoRepository(DsGestorDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<CoteFacilProdutoView> Dados, int Total)> ConsultarAsync(
        string cnpjDistribuidor,
        IEnumerable<int> codigosProduto,
        IEnumerable<string> eans,
        IEnumerable<string> duns,
        string? descricao,
        int page,
        int size)
    {
        var codigosList = codigosProduto.Distinct().ToList();
        var eansList = eans.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
        var dunsList = duns.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

        var query = _context.CoteFacilProdutos
            .AsNoTracking()
            .Where(x => x.CnpjDistribuidor == cnpjDistribuidor);

        var possuiFiltroProduto =
            codigosList.Count > 0 ||
            eansList.Count > 0 ||
            dunsList.Count > 0;

        if (possuiFiltroProduto)
        {
            query = query.Where(x =>
                codigosList.Contains(x.CodigoProduto) ||
                (x.Ean != null && eansList.Contains(x.Ean)) ||
                (x.Dun != null && dunsList.Contains(x.Dun)));
        }

        if (!string.IsNullOrWhiteSpace(descricao))
        {
            var desc = descricao.Trim().ToUpper();

            query = query.Where(x =>
                x.Descricao != null &&
                x.Descricao.ToUpper().Contains(desc));
        }

        var total = await query.CountAsync();

        var dados = await query
            .OrderBy(x => x.Descricao)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();

        return (dados, total);
    }
}