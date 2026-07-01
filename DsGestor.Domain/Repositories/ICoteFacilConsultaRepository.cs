using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface ICoteFacilConsultaRepository
{
    Task<IEnumerable<CoteFacilFilialDistribuidor>> GetFiliaisAsync();

    Task<(IEnumerable<Cliente> Dados, int Total)> GetClientesAsync(
        string? cnpjDistribuidor,
        string? cnpjCliente,
        int page,
        int size);

    Task<(IEnumerable<CoteFacilCondicaoPagamento> Dados, int Total)> GetCondicoesPagamentoAsync(
        string? cnpjDistribuidor,
        int? codigoCondicaoPagamento,
        int page,
        int size);
}