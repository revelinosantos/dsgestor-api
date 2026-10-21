using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;

namespace DsGestor.Domain.Repositories;

public interface IOcorrenciaRepository
{
    Task<IEnumerable<Ocorrencia>> GetAllAsync(CancellationToken ct = default);
    Task<Ocorrencia?> GetByIdAsync(int id, bool incluirItens = false, CancellationToken ct = default);

    Task<Ocorrencia?> GetDraftAsync(
        string codigoFilial,
        decimal numeroNota,
        string serieNota,
        int codigoUsuario,
        bool incluirItens = true,
        CancellationToken ct = default);

    Task<Ocorrencia> CreateOrGetDraftAsync(Ocorrencia ocorrencia, CancellationToken ct = default);
    Task UpdateAsync(Ocorrencia ocorrencia, CancellationToken ct = default);
    Task<bool> FinalizarRascunhoAsync(int id, int codigoUsuario, CancellationToken ct = default);

    Task UpdateStatusAsync(
        int id,
        StatusOcorrencia status,
        int codigoUsuario,
        string? observacao = null,
        CancellationToken ct = default);

    Task ConfirmarCreditoAsync(int id, decimal? numeroCredito, decimal? numeroNotaCredito,
        int codigoUsuario, string? observacao = null, CancellationToken ct = default);
    Task CancelarAsync(int id, int codigoUsuario, string motivo, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<Ocorrencia> CreateWithItemsAsync(
    Ocorrencia ocorrencia,
    CancellationToken ct = default);
}
