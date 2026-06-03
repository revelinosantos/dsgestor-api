using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> GetAllAsync();
    Task<Produto?> GetByIdAsync(int id);
    Task<IEnumerable<Produto>> GetByFilialAsync(string codigoFilial);

    Task<IEnumerable<Produto>> GetByFilialAndRegiaoAsync(string codigoFilial, int codigoRegiao);

    Task<IEnumerable<Produto>> GetByFilialAndRegiaoAndDescricaoAsync(
        string codigoFilial,
        int codigoRegiao,
        string descricao);

    Task<Produto?> GetByFilialAndRegiaoByIdAsync(
        string codigoFilial,
        int codigoRegiao,
        int id);
}
