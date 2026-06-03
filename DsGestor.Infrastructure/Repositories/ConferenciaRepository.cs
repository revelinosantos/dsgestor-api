using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Repositories;

public class ConferenciaRepository : IConferenciaRepository
{
    private readonly DsGestorDbContext _context;

    public ConferenciaRepository(DsGestorDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExisteConferenciaAsync(decimal numped, int numnota)
    {
        return await _context.Conferencias
            .AsNoTracking()
            .AnyAsync(x => x.Numped == numped && x.Numnota == numnota);
    }

    public async Task<List<Conferencia>> ObterItensAsync(decimal numped, int numnota)
    {
        return await _context.Conferencias
            .Where(x => x.Numped == numped && x.Numnota == numnota)
            .OrderBy(x => x.Numseq)
            .ToListAsync();
    }

    public async Task<Conferencia?> ObterItemPorIdAsync(int confid)
    {
        return await _context.Conferencias
            .FirstOrDefaultAsync(x => x.Confid == confid);
    }

    public async Task<Conferencia?> BuscarItemPorEanAsync(decimal numped, int numnota, string ean)
    {
        ean = NormalizarCodigo(ean);

        return await _context.Conferencias
            .Where(x => x.Numped == numped && x.Numnota == numnota)
            .Where(x =>
                x.Codauxiliar == ean ||
                x.Codauxiliar2 == ean ||
                x.Lpvcod == ean ||
                x.Lpvcodfind == ean)
            .OrderBy(x => x.Numseq)
            .FirstOrDefaultAsync();
    }

    public async Task ImportarPedidoAsync(decimal numped, int numnota, int codUsuario)
    {
        var itensView = await _context.ConferenciaPedidoView
            .AsNoTracking()
            .Where(x => x.Numped == numped)
            .OrderBy(x => x.Numseq)
            .ToListAsync();

        if (!itensView.Any())
            throw new Exception($"Pedido {numped} não encontrado na VIEW_DSGESTOR_CONF_PED.");

        var agora = DateTime.Now;

        var itens = itensView.Select(x =>
        {
            var qtOriginal = x.Qt ?? x.Qtoriginal ?? 0;
            var nota = numnota > 0 ? numnota : (x.Numnota ?? 0);

            return new Conferencia
            {
                // CONFID não informa. Oracle gera automaticamente.

                Numped = x.Numped,
                Dataped = x.Dataped,
                Statusped = x.Statusped,
                Codfilial = x.Codfilial,
                Codusur = x.Codusur,
                Condvenda = x.Condvenda,
                Codemitenteped = x.Codemitenteped,
                Codcli = x.Codcli,
                Cliente = x.Cliente,
                Vltotal = x.Vltotal,

                Codprod = x.Codprod,
                Numseq = x.Numseq,
                Descricao = x.Descricao,

                Qtoriginal = qtOriginal,
                Qt = qtOriginal,
                Qtfalta = qtOriginal,
                Qtunit = x.Qtunit,
                Qtunitcx = x.Qtunitcx,

                Pvenda = x.Pvenda,
                Codauxiliar = x.Codauxiliar,

                Pesobrutopro = x.Pesobrutopro,
                Pesoliqpro = x.Pesoliqpro,

                Codconf = x.Codconf,
                Datainiconf = agora,
                Datafimconf = null,

                Qtconfunid = 0,
                Qtconf = 0,

                Conferido = "N",
                Conferidoitem = "N",

                Volume = x.Volume,

                Pesobrutoped = x.Pesobrutoped,
                Pesoliqped = x.Pesoliqped,

                Fechadodiverg = "N",
                Datafechdiverg = null,

                Codsep = x.Codsep,
                Dataconfwinthor = null,
                Nomeconf = x.Nomeconf,

                Codfundiverg = 0,

                Codauxiliar2 = x.Codauxiliar2,

                Android = "S",

                Numnota = nota,
                Numtransvenda = x.Numtransvenda ?? 0,

                Lpvid = x.Lpvid ?? 0,
                Lpvcod = string.IsNullOrWhiteSpace(x.Lpvcod) ? "N" : x.Lpvcod,
                Lpvcodfind = string.IsNullOrWhiteSpace(x.Lpvcodfind) ? "N" : x.Lpvcodfind,

                Statusconf = "ABERTA",
                Usuarioinicio = codUsuario > 0 ? codUsuario : null,
                Usuariofinalizou = null,
                Dataultleitura = null,
                Observacao = null,
                Finalizadowinthor = "N",
                Errowinthor = null
            };
        }).ToList();

        await _context.Conferencias.AddRangeAsync(itens);
        await _context.SaveChangesAsync();
    }

    public async Task FinalizarPedidoAsync(
        decimal numped,
        int numnota,
        int codUsuario,
        bool finalizarComDivergencia = false,
        int? codSupervisor = null,
        string? observacao = null)
    {
        var itens = await _context.Conferencias
            .Where(x => x.Numped == numped && x.Numnota == numnota)
            .OrderBy(x => x.Numseq)
            .ToListAsync();

        if (!itens.Any())
            throw new Exception($"Pedido {numped} não encontrado para conferência.");

        if (itens.Any(x => (x.Numtransvenda ?? 0) > 0))
            throw new Exception("Este pedido já possui NUMTRANSVENDA. A finalização de conferência após emissão da nota ainda não foi implementada.");

        var agora = DateTime.Now;
        var primeiro = itens.First();

        var volume = primeiro.Volume ?? 0;
        var pesoLiquidoPedido = primeiro.Pesoliqped ?? 0;

        var codConf = codUsuario > 0
            ? codUsuario
            : primeiro.Codconf ?? 0;

        var houveDivergencia = false;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var item in itens)
            {
                var qtOriginal = ObterQuantidadeOriginal(item);
                var qtConf = item.Qtconfunid ?? item.Qtconf ?? 0;
                var qtFalta = Math.Max(qtOriginal - qtConf, 0);

                item.Qt = qtOriginal;
                item.Qtoriginal = qtOriginal;
                item.Qtconfunid = qtConf;
                item.Qtconf = qtConf;
                item.Qtfalta = qtFalta;
                item.Conferidoitem = qtConf >= qtOriginal ? "S" : "N";

                var codfilial = item.Codfilial ?? primeiro.Codfilial ?? 0;
                var codprod = item.Codprod;
                var numseq = item.Numseq;
                var codsep = item.Codsep ?? 0;
                var codcli = item.Codcli ?? primeiro.Codcli ?? 0;
                var codusur = item.Codusur ?? primeiro.Codusur ?? 0;
                var condvenda = item.Condvenda ?? primeiro.Condvenda ?? 0;
                var codemitenteped = item.Codemitenteped ?? primeiro.Codemitenteped ?? 0;
                var pvenda = item.Pvenda ?? 0;

                if (qtFalta > 0)
                {
                    houveDivergencia = true;

                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        UPDATE PCEST
                           SET QTRESERV = NVL(QTRESERV, 0) - {qtFalta}
                         WHERE CODFILIAL = {codfilial}
                           AND CODPROD = {codprod}
                    ");

                    var motivo = item.Conferidoitem == "N"
                        ? "FALTA DE MERCADORIA"
                        : "CORTE DO ITEM";

                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        INSERT INTO PCCORTEI
                        (
                            CODPROD,
                            QTSEPARADA,
                            QTCORTADA,
                            DATA,
                            NUMCAR,
                            CODFUNC,
                            NUMPED,
                            PVENDA,
                            CODFUNCCONF,
                            CODFUNCSEP,
                            CODFILIAL,
                            QTORIG,
                            QTFALTA,
                            MOTIVO,
                            DTFINALCHECKOUT,
                            HORA,
                            MINUTO,
                            CODCLI,
                            CODUSUR,
                            CODROTINA,
                            CONDVENDA,
                            CODEMITENTEPED,
                            NUMSEQ
                        )
                        VALUES
                        (
                            {codprod},
                            {qtConf},
                            {qtFalta},
                            SYSDATE,
                            0,
                            {codConf},
                            {numped},
                            {pvenda},
                            {codConf},
                            {codsep},
                            {codfilial},
                            {qtOriginal},
                            0,
                            {motivo},
                            SYSDATE,
                            TO_NUMBER(TO_CHAR(SYSDATE, 'HH24')),
                            TO_NUMBER(TO_CHAR(SYSDATE, 'MI')),
                            {codcli},
                            {codusur},
                            936,
                            {condvenda},
                            {codemitenteped},
                            {numseq}
                        )
                    ");
                }

                if (item.Conferidoitem == "N")
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        DELETE FROM PCPEDI
                         WHERE NUMPED = {numped}
                           AND CODPROD = {codprod}
                           AND NUMSEQ = {numseq}
                    ");
                }
                else
                {
                    var corte = qtFalta > 0 ? "S" : "N";

                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        UPDATE PCPEDI
                           SET QT = {qtConf},
                               QTSEPARADA = {qtConf},
                               QTFALTA = {qtFalta},
                               CODFUNCSEP = {codsep},
                               CODFUNCCONF = {codConf},
                               QTORIGINAL = {qtOriginal},
                               DATACONF = {item.Datainiconf ?? agora},
                               DATACONFFIM = {agora},
                               CORTE = {corte}
                         WHERE NUMPED = {numped}
                           AND CODPROD = {codprod}
                           AND NUMSEQ = {numseq}
                    ");
                }
            }

            if (houveDivergencia)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    UPDATE PCPEDC pdv
                       SET VLTABELA = ROUND((
                                SELECT NVL(SUM(ipd.PVENDA * ipd.QT), 0)
                                  FROM PCPEDI ipd
                                 WHERE ipd.NUMPED = pdv.NUMPED
                           ), 2),
                           VLCUSTOREAL = ROUND((
                                SELECT NVL(SUM(ipd.VLCUSTOREAL * ipd.QT), 0)
                                  FROM PCPEDI ipd
                                 WHERE ipd.NUMPED = pdv.NUMPED
                           ), 2),
                           VLCUSTOFIN = ROUND((
                                SELECT NVL(SUM(ipd.VLCUSTOFIN * ipd.QT), 0)
                                  FROM PCPEDI ipd
                                 WHERE ipd.NUMPED = pdv.NUMPED
                           ), 2),
                           VLATEND = ROUND((
                                SELECT NVL(SUM(ipd.PVENDA * ipd.QT), 0)
                                  FROM PCPEDI ipd
                                 WHERE ipd.NUMPED = pdv.NUMPED
                           ), 2),
                           VLCUSTOREP = ROUND((
                                SELECT NVL(SUM(ipd.VLCUSTOREP * ipd.QT), 0)
                                  FROM PCPEDI ipd
                                 WHERE ipd.NUMPED = pdv.NUMPED
                           ), 2),
                           VLCUSTOCONT = ROUND((
                                SELECT NVL(SUM(ipd.VLCUSTOCONT * ipd.QT), 0)
                                  FROM PCPEDI ipd
                                 WHERE ipd.NUMPED = pdv.NUMPED
                           ), 2),
                           NUMITENS = (
                                SELECT COUNT(*)
                                  FROM PCPEDI ipd
                                 WHERE ipd.NUMPED = pdv.NUMPED
                           ),
                           DTINICIALCHECKOUT = SYSDATE,
                           DTFINALCHECKOUT = SYSDATE,
                           TOTVOLUME = {volume},
                           TOTPESO = {pesoLiquidoPedido},
                           CODFUNCCONF = {codConf},
                           NUMVOLUME = {volume}
                     WHERE pdv.NUMPED = {numped}
                ");
            }
            else
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    UPDATE PCPEDC
                       SET DTINICIALCHECKOUT = SYSDATE,
                           DTFINALCHECKOUT = SYSDATE,
                           TOTVOLUME = {volume},
                           TOTPESO = {pesoLiquidoPedido},
                           CODFUNCCONF = {codConf},
                           NUMVOLUME = {volume}
                     WHERE NUMPED = {numped}
                ");
            }

            foreach (var item in itens)
            {
                item.Conferido = "S";
                item.Datafimconf = agora;
                item.Usuariofinalizou = codUsuario;
                item.Dataconfwinthor = agora;
                item.Finalizadowinthor = "S";
                item.Errowinthor = null;

                if (houveDivergencia || finalizarComDivergencia)
                {
                    item.Fechadodiverg = "S";
                    item.Codfundiverg = codSupervisor ?? item.Codfundiverg ?? 0;
                    item.Datafechdiverg = agora;
                    item.Observacao = observacao ?? item.Observacao;
                    item.Statusconf = "FINALIZADA_DIVERG";
                }
                else
                {
                    item.Fechadodiverg = "N";
                    item.Codfundiverg = 0;
                    item.Datafechdiverg = null;
                    item.Statusconf = "FINALIZADA";
                }
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            await MarcarErroAsync(numped, numnota, ex.Message);

            throw;
        }
    }

    public async Task AdicionarItensAsync(IEnumerable<Conferencia> itens)
    {
        await _context.Conferencias.AddRangeAsync(itens);
    }

    public Task AtualizarAsync(Conferencia item)
    {
        _context.Conferencias.Update(item);
        return Task.CompletedTask;
    }

    public Task AtualizarItensAsync(IEnumerable<Conferencia> itens)
    {
        _context.Conferencias.UpdateRange(itens);
        return Task.CompletedTask;
    }

    public async Task SalvarAsync()
    {
        await _context.SaveChangesAsync();
    }

    private async Task MarcarErroAsync(decimal numped, int numnota, string erro)
    {
        try
        {
            _context.ChangeTracker.Clear();

            var itens = await _context.Conferencias
                .Where(x => x.Numped == numped && x.Numnota == numnota)
                .ToListAsync();

            foreach (var item in itens)
            {
                item.Statusconf = "ERRO_WINTHOR";
                item.Finalizadowinthor = "N";
                item.Errowinthor = erro.Length > 1000
                    ? erro.Substring(0, 1000)
                    : erro;
            }

            await _context.SaveChangesAsync();
        }
        catch
        {
            // Não relança aqui para não esconder o erro original da finalização.
        }
    }

    private static int ObterQuantidadeOriginal(Conferencia item)
    {
        return item.Qt ?? item.Qtoriginal ?? 0;
    }

    private static string NormalizarCodigo(string valor)
    {
        return valor.Trim();
    }
}