using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface IConferenciaRepository
{
    Task<bool> ExisteConferenciaAsync(decimal numped, int numnota);

    Task<List<Conferencia>> ObterItensAsync(decimal numped, int numnota);

    Task<Conferencia?> ObterItemPorIdAsync(int confid);

    Task<Conferencia?> BuscarItemPorEanAsync(decimal numped, int numnota, string ean);

    Task AdicionarItensAsync(IEnumerable<Conferencia> itens);

    Task AtualizarAsync(Conferencia item);

    Task AtualizarItensAsync(IEnumerable<Conferencia> itens);

    Task SalvarAsync();
    Task ImportarPedidoAsync(decimal numped, int numnota, int codUsuario);
    Task FinalizarPedidoAsync( decimal numped,
         int numnota,
         int codUsuario,
         bool finalizarComDivergencia = false,
         int? codSupervisor = null,
         string? observacao = null);
}