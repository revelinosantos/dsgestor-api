using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories;

public interface ILancamentoDetRepository
{
    Task<IEnumerable<LancamentoDet>> GetByLancamentoAsync(int idLanc);
    Task<LancamentoDet?> GetByIdAsync(int id);

    Task<LancamentoDet> CreateAsync(LancamentoDet det);

    // Atualização normal (RASCUNHO)
    Task UpdateAsync(LancamentoDet det);

    // Atualização de autorização (PENDENTE por ADMIN/GERENTE)
    Task UpdateAutorizacaoAsync(LancamentoDet det, int codigoUsuarioAut);

    Task DeleteAsync(int id);
}