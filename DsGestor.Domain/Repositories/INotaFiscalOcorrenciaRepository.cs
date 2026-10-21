using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface INotaFiscalOcorrenciaRepository
{
    Task<IReadOnlyList<NotaFiscalOcorrenciaView>> PesquisarAsync(
        decimal numeroNota, string codigoFilial, string serie, CancellationToken ct = default);
    Task<bool> ItemPossuiOcorrenciaAsync(decimal numeroTransacaoVenda, int numeroTransacaoItem,
        CancellationToken ct = default);
}
