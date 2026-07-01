using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface ICoteFacilPedidoRepository
{
    Task<CoteFacilPedido?> GetPedidoExistenteAsync(
        string cnpjDistribuidor,
        string cnpjCliente,
        long? cotacaoCoteFacil,
        long? pedidoCoteFacil);

    Task<CoteFacilPedido?> GetByIdAsync(long idPedido, bool incluirItens = false);

    Task<CoteFacilPedido> AddAsync(CoteFacilPedido pedido);

    Task AtualizarRetornoAsync(
        long idPedido,
        string status,
        string jsonResponse,
        string? mensagemErro);

    Task<IEnumerable<CoteFacilPedido>> GetConfirmacaoAsync(
        string cnpjDistribuidor,
        DateTime? dataInicial,
        DateTime? dataFinal,
        IEnumerable<string> pedidos);

    Task<long> ImportarPedidoWinThorAsync(long idPedido, CancellationToken ct);       
}