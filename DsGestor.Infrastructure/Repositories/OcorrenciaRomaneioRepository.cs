using System.Data;
using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public sealed class OcorrenciaRomaneioRepository : IOcorrenciaRomaneioRepository
{
    private readonly DsGestorDbContext _context;

    public OcorrenciaRomaneioRepository(DsGestorDbContext context) =>
        _context = context;

    public async Task<IEnumerable<OcorrenciaRomaneio>> GetAllAsync(
        string? codigoFilial = null,
        int? codigoMotorista = null,
        StatusRomaneioOcorrencia? status = null,
        CancellationToken ct = default)
    {
        var query = BaseQuery(incluirDetalhesOcorrencias: false);

        if (!string.IsNullOrWhiteSpace(codigoFilial))
            query = query.Where(x => x.CodigoFilial == codigoFilial.Trim());

        if (codigoMotorista.HasValue)
            query = query.Where(x => x.CodigoMotorista == codigoMotorista.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        return await query
            .OrderByDescending(x => x.Data)
            .ThenByDescending(x => x.Id)
            .ToListAsync(ct);
    }

    public Task<OcorrenciaRomaneio?> GetByIdAsync(
        int id,
        CancellationToken ct = default) =>
        BaseQuery(incluirDetalhesOcorrencias: true)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IEnumerable<Ocorrencia>> GetOcorrenciasPendentesAsync(
        int codigoMotorista,
        string? codigoFilial = null,
        CancellationToken ct = default)
    {
        var query = _context.Ocorrencias
            .AsNoTrackingWithIdentityResolution()
            .Include(x => x.Cliente)
            .Include(x => x.Motorista)
            .Include(x => x.Itens)
                .ThenInclude(x => x.Produto)
            .Include(x => x.Itens)
                .ThenInclude(x => x.Motivo)
            .Where(x =>
                x.CodigoMotorista == codigoMotorista &&
                x.Status == StatusOcorrencia.ABERTA &&
                !x.Romaneios.Any(vinculo =>
                    vinculo.Romaneio != null &&
                    vinculo.Romaneio.Status == StatusRomaneioOcorrencia.ATIVO));

        if (!string.IsNullOrWhiteSpace(codigoFilial))
            query = query.Where(x => x.CodigoFilial == codigoFilial.Trim());

        return await query
            .OrderBy(x => x.Data)
            .ThenBy(x => x.Id)
            .AsSplitQuery()
            .ToListAsync(ct);
    }

    public async Task<OcorrenciaRomaneio> CreateAsync(
        OcorrenciaRomaneio romaneio,
        IReadOnlyCollection<int> codigosOcorrencias,
        CancellationToken ct = default)
    {
        ValidarCabecalho(romaneio);

        var codigos = codigosOcorrencias
            .Where(x => x > 0)
            .Distinct()
            .ToArray();

        if (codigos.Length == 0)
            throw new InvalidOperationException(
                "Selecione pelo menos uma ocorrência para o romaneio.");

        if (codigos.Length != codigosOcorrencias.Count)
            throw new InvalidOperationException(
                "A lista contém ocorrências inválidas ou repetidas.");

        await using var tx = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, ct);

        try
        {
            var ocorrencias = await _context.Ocorrencias
                .Include(x => x.Romaneios)
                    .ThenInclude(x => x.Romaneio)
                .Where(x => codigos.Contains(x.Id))
                .ToListAsync(ct);

            if (ocorrencias.Count != codigos.Length)
            {
                var encontrados = ocorrencias.Select(x => x.Id).ToHashSet();
                var inexistentes = codigos.Where(x => !encontrados.Contains(x));
                throw new KeyNotFoundException(
                    $"Ocorrência(s) não encontrada(s): {string.Join(", ", inexistentes)}.");
            }

            foreach (var ocorrencia in ocorrencias)
                ValidarOcorrenciaParaRomaneio(ocorrencia, romaneio);

            var agora = DateTime.Now;

            romaneio.Id = 0;
            romaneio.Data = agora;
            romaneio.Status = StatusRomaneioOcorrencia.ATIVO;
            romaneio.DataCancelamento = null;
            romaneio.CodigoUsuarioCancelamento = null;
            romaneio.MotivoCancelamento = null;
            romaneio.Ocorrencias.Clear();

            // Primeiro grava somente o cabeçalho para que o trigger do Oracle
            // gere CODROMANEIO. Tudo continua dentro da mesma transação.
            _context.OcorrenciasRomaneios.Add(romaneio);
            await _context.SaveChangesAsync(ct);

            foreach (var ocorrencia in ocorrencias)
            {
                romaneio.Ocorrencias.Add(new OcorrenciaRomaneioOcorrencia
                {
                    CodigoOcorrencia = ocorrencia.Id,
                    Ocorrencia = ocorrencia
                });

                ocorrencia.Status = StatusOcorrencia.ENTREGUE_MOTORISTA;
                ocorrencia.DataEntregaMotorista = agora;
                ocorrencia.CodigoUsuarioEntrega = romaneio.CodigoUsuario;

                AddHistorico(
                    ocorrencia.Id,
                    romaneio.CodigoUsuario,
                    StatusOcorrencia.ABERTA,
                    StatusOcorrencia.ENTREGUE_MOTORISTA,
                    $"Ocorrência entregue ao motorista pelo romaneio #{romaneio.Id}.",
                    agora);
            }

            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return await GetByIdAsync(romaneio.Id, ct) ?? romaneio;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task CancelarAsync(
        int id,
        int codigoUsuario,
        string motivo,
        CancellationToken ct = default)
    {
        if (codigoUsuario <= 0)
            throw new InvalidOperationException("Usuário não identificado.");

        if (string.IsNullOrWhiteSpace(motivo))
            throw new InvalidOperationException(
                "Informe o motivo do cancelamento do romaneio.");

        if (motivo.Trim().Length > 500)
            throw new InvalidOperationException(
                "Motivo do cancelamento deve possuir no máximo 500 caracteres.");

        await using var tx = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, ct);

        try
        {
            var romaneio = await _context.OcorrenciasRomaneios
                .Include(x => x.Ocorrencias)
                    .ThenInclude(x => x.Ocorrencia)
                .FirstOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new KeyNotFoundException("Romaneio não encontrado.");

            if (romaneio.Status == StatusRomaneioOcorrencia.CANCELADO)
                throw new InvalidOperationException("O romaneio já está cancelado.");

            if (romaneio.Ocorrencias.Count == 0)
                throw new InvalidOperationException(
                    "O romaneio não possui ocorrências vinculadas.");

            var ocorrenciasInvalidas = romaneio.Ocorrencias
                .Where(x => x.Ocorrencia?.Status != StatusOcorrencia.ENTREGUE_MOTORISTA)
                .Select(x => x.CodigoOcorrencia)
                .ToArray();

            if (ocorrenciasInvalidas.Length > 0)
            {
                throw new InvalidOperationException(
                    "Não é possível cancelar o romaneio porque a(s) ocorrência(s) " +
                    $"{string.Join(", ", ocorrenciasInvalidas)} já avançaram no processo.");
            }

            var agora = DateTime.Now;
            var motivoLimpo = motivo.Trim();

            romaneio.Status = StatusRomaneioOcorrencia.CANCELADO;
            romaneio.DataCancelamento = agora;
            romaneio.CodigoUsuarioCancelamento = codigoUsuario;
            romaneio.MotivoCancelamento = motivoLimpo;

            foreach (var vinculo in romaneio.Ocorrencias)
            {
                var ocorrencia = vinculo.Ocorrencia!;
                ocorrencia.Status = StatusOcorrencia.ABERTA;
                ocorrencia.DataEntregaMotorista = null;
                ocorrencia.CodigoUsuarioEntrega = null;

                AddHistorico(
                    ocorrencia.Id,
                    codigoUsuario,
                    StatusOcorrencia.ENTREGUE_MOTORISTA,
                    StatusOcorrencia.ABERTA,
                    $"Romaneio #{romaneio.Id} cancelado. Motivo: {motivoLimpo}",
                    agora);
            }

            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private IQueryable<OcorrenciaRomaneio> BaseQuery(
        bool incluirDetalhesOcorrencias)
    {
        IQueryable<OcorrenciaRomaneio> query = _context.OcorrenciasRomaneios
            .AsNoTrackingWithIdentityResolution()
            .Include(x => x.Motorista)
            .Include(x => x.Usuario)
            .Include(x => x.UsuarioCancelamento);

        if (incluirDetalhesOcorrencias)
        {
            query = query
                .Include(x => x.Ocorrencias)
                    .ThenInclude(x => x.Ocorrencia)
                        .ThenInclude(x => x!.Cliente)
                .Include(x => x.Ocorrencias)
                    .ThenInclude(x => x.Ocorrencia)
                        .ThenInclude(x => x!.Itens)
                            .ThenInclude(x => x.Produto)
                .Include(x => x.Ocorrencias)
                    .ThenInclude(x => x.Ocorrencia)
                        .ThenInclude(x => x!.Itens)
                            .ThenInclude(x => x.Motivo)
                .Include(x => x.Ocorrencias)
                    .ThenInclude(x => x.Ocorrencia)
                        .ThenInclude(x => x!.Motorista);
        }
        else
        {
            query = query.Include(x => x.Ocorrencias);
        }

        return query.AsSplitQuery();
    }

    private void AddHistorico(
        int codigoOcorrencia,
        int codigoUsuario,
        StatusOcorrencia statusAnterior,
        StatusOcorrencia statusNovo,
        string observacao,
        DateTime data) =>
        _context.OcorrenciasHistorico.Add(new OcorrenciaHistorico
        {
            CodigoOcorrencia = codigoOcorrencia,
            Data = data,
            CodigoUsuario = codigoUsuario,
            StatusAnterior = statusAnterior,
            StatusNovo = statusNovo,
            Observacao = observacao
        });

    private static void ValidarCabecalho(OcorrenciaRomaneio romaneio)
    {
        if (string.IsNullOrWhiteSpace(romaneio.CodigoFilial))
            throw new InvalidOperationException("Informe a filial.");

        if (romaneio.CodigoFilial.Trim().Length > 2)
            throw new InvalidOperationException("Código da filial deve possuir no máximo 2 caracteres.");

        if (romaneio.CodigoMotorista <= 0)
            throw new InvalidOperationException("Informe o motorista.");

        if (romaneio.CodigoUsuario <= 0)
            throw new InvalidOperationException("Usuário não identificado.");

        romaneio.CodigoFilial = romaneio.CodigoFilial.Trim();
        romaneio.Observacao = Limpar(romaneio.Observacao);

        if (romaneio.Observacao?.Length > 1000)
            throw new InvalidOperationException("Observação deve possuir no máximo 1000 caracteres.");
    }

    private static void ValidarOcorrenciaParaRomaneio(
        Ocorrencia ocorrencia,
        OcorrenciaRomaneio romaneio)
    {
        if (ocorrencia.Status != StatusOcorrencia.ABERTA)
        {
            throw new InvalidOperationException(
                $"A ocorrência #{ocorrencia.Id} não está ABERTA.");
        }

        if (ocorrencia.CodigoMotorista != romaneio.CodigoMotorista)
        {
            throw new InvalidOperationException(
                $"A ocorrência #{ocorrencia.Id} pertence a outro motorista.");
        }

        if (!string.Equals(
                ocorrencia.CodigoFilial,
                romaneio.CodigoFilial,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"A ocorrência #{ocorrencia.Id} pertence a outra filial.");
        }

        var possuiRomaneioAtivo = ocorrencia.Romaneios.Any(x =>
            x.Romaneio?.Status == StatusRomaneioOcorrencia.ATIVO);

        if (possuiRomaneioAtivo)
        {
            throw new InvalidOperationException(
                $"A ocorrência #{ocorrencia.Id} já pertence a um romaneio ativo.");
        }
    }

    private static string? Limpar(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
