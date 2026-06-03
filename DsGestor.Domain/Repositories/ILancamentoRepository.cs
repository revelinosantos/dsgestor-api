using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;

namespace DsGestor.Domain.Repositories;

public interface ILancamentoRepository
{
    Task<IEnumerable<Lancamento>> GetAllAsync();
    Task<Lancamento?> GetByIdAsync(int id, bool incluirItens = false);
    Task<IEnumerable<Lancamento>> GetByUsuarioAsync(int codigoUsuario);
    Task<IEnumerable<Lancamento>> GetByStatusAsync(StatusLancamento status);

    // ✅ NOVO: rascunho
    Task<Lancamento?> GetDraftAsync(string codigoFilial, int numRegiao, int CodigoVendedor,int codigoCliente, int codigoUsuario, bool incluirItens = true);
    Task<Lancamento> CreateOrGetDraftAsync(Lancamento draft);

    // ✅ NOVO: finalizar rascunho
    Task<bool> FinalizarAsync(int id);

    Task<Lancamento> CreateAsync(Lancamento entity);
    Task UpdateAsync(Lancamento entity);
    Task UpdateStatusAsync(int id, StatusLancamento status, int? codigoUsuarioAut);
    Task DeleteAsync(int id);

    // ✅ NOVO: utilitário para validação (usado no Det)
    Task<StatusLancamento?> GetStatusAsync(int id);
    Task<int> AutorizarAsync(int id, int codigoUsuarioAut, CancellationToken ct);
}
