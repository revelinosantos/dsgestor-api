using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface ICoteFacilProdutoRepository
{
    Task<(IEnumerable<CoteFacilProdutoView> Dados, int Total)> ConsultarAsync(
        string cnpjDistribuidor,
        IEnumerable<int> codigosProduto,
        IEnumerable<string> eans,
        IEnumerable<string> duns,
        string? descricao,
        int page,
        int size);
}