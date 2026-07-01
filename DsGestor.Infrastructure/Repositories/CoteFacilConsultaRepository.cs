using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public class CoteFacilConsultaRepository : ICoteFacilConsultaRepository
{
    private readonly DsGestorDbContext _context;

    public CoteFacilConsultaRepository(DsGestorDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CoteFacilFilialDistribuidor>> GetFiliaisAsync()
    {
        return await _context.CoteFacilFiliaisDistribuidor
            .AsNoTracking()
            .Where(x => !string.IsNullOrWhiteSpace(x.Cnpj))
            .OrderBy(x => x.CodigoFilial)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Cliente> Dados, int Total)> GetClientesAsync(
        string? cnpjDistribuidor,
        string? cnpjCliente,
        int page,
        int size)
    {
        var query = _context.Clientes
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(cnpjCliente))
        {
            query = query.Where(x =>
                x.NumRegiao == 1 &&
                x.CnpjCpf != null &&
                x.CnpjCpf
                    .Replace(".", "")
                    .Replace("/", "")
                    .Replace("-", "") == cnpjCliente);
        }

        var total = await query.CountAsync();

        var dados = await query
            .OrderBy(x => x.RazaoSocial)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();

        return (dados, total);
    }

    public async Task<(IEnumerable<CoteFacilCondicaoPagamento> Dados, int Total)> GetCondicoesPagamentoAsync(
        string? cnpjDistribuidor,
        int? codigoCondicaoPagamento,
        int page,
        int size)
    {
        var query = _context.CoteFacilCondicoesPagamento
            .AsNoTracking()
            .AsQueryable();

        if (codigoCondicaoPagamento.HasValue)
        {
            query = query.Where(x =>
                x.CodigoCondicaoPagamento == codigoCondicaoPagamento.Value);
        }

        var total = await query.CountAsync();

        var dados = await query
            .OrderBy(x => x.CodigoCondicaoPagamento)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();

        return (dados, total);
    }
}