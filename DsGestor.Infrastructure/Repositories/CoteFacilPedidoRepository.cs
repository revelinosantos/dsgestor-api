using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace DsGestor.Infrastructure.Repositories;

public class CoteFacilPedidoRepository : ICoteFacilPedidoRepository
{
    private readonly DsGestorDbContext _context;

    public CoteFacilPedidoRepository(DsGestorDbContext context)
    {
        _context = context;
    }

    public async Task<CoteFacilPedido?> GetPedidoExistenteAsync(
        string cnpjDistribuidor,
        string cnpjCliente,
        long? cotacaoCoteFacil,
        long? pedidoCoteFacil)
    {
        return await _context.CoteFacilPedidos
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.CnpjDistribuidor == cnpjDistribuidor &&
                x.CnpjCliente == cnpjCliente &&
                x.CotacaoCoteFacil == cotacaoCoteFacil &&
                x.PedidoCoteFacil == pedidoCoteFacil);
    }

    public async Task<CoteFacilPedido?> GetByIdAsync(long idPedido, bool incluirItens = false)
    {
        var query = _context.CoteFacilPedidos.AsQueryable();

        if (incluirItens)
            query = query.Include(x => x.Itens);

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdPedido == idPedido);
    }

    public async Task<CoteFacilPedido> AddAsync(CoteFacilPedido pedido)
    {
        _context.CoteFacilPedidos.Add(pedido);
        await _context.SaveChangesAsync();

        return pedido;
    }

    public async Task AtualizarRetornoAsync(
        long idPedido,
        string status,
        string jsonResponse,
        string? mensagemErro)
    {
        var pedido = await _context.CoteFacilPedidos
            .FirstOrDefaultAsync(x => x.IdPedido == idPedido);

        if (pedido is null)
            return;

        pedido.Status = status;
        pedido.JsonResponse = jsonResponse;
        pedido.MensagemErro = mensagemErro;
        pedido.DataAtualizacao = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CoteFacilPedido>> GetConfirmacaoAsync(
        string cnpjDistribuidor,
        DateTime? dataInicial,
        DateTime? dataFinal,
        IEnumerable<string> pedidos)
    {
        var query = _context.CoteFacilPedidos
            .AsNoTracking()
            .AsQueryable();

        query = query.Where(x => x.CnpjDistribuidor == cnpjDistribuidor);

        if (dataInicial.HasValue)
            query = query.Where(x => x.DataCriacao >= dataInicial.Value.Date);

        if (dataFinal.HasValue)
        {
            var dataFinalExclusive = dataFinal.Value.Date.AddDays(1);
            query = query.Where(x => x.DataCriacao < dataFinalExclusive);
        }

        var pedidosLong = pedidos
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => long.TryParse(x, out var n) ? n : (long?)null)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .ToList();

        if (pedidosLong.Count > 0)
        {
            query = query.Where(x =>
                x.PedidoCoteFacil.HasValue &&
                pedidosLong.Contains(x.PedidoCoteFacil.Value));
        }

        return await query
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync();
    }

    public async Task<long> ImportarPedidoWinThorAsync(long idPedido, CancellationToken ct)
    {
        var pedido = await _context.CoteFacilPedidos
            .Include(x => x.Itens)
            .FirstOrDefaultAsync(x => x.IdPedido == idPedido, ct);

        if (pedido is null)
            throw new InvalidOperationException("Pedido Cote Fácil não encontrado.");

        if (pedido.Status == "IMPORTADO" && pedido.NumpedWinthor.HasValue)
            return pedido.NumpedWinthor.Value;

        ValidarPedidoAntesImportacao(pedido);

        await using var trx = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            pedido.Status = "IMPORTANDO_WINTHOR";
            pedido.TentativasProcessamento += 1;
            pedido.DataAtualizacao = DateTime.Now;
            pedido.MensagemErro = null;

            await _context.SaveChangesAsync(ct);

            var numped = await ObterProximoNumpedAsync(ct);

            var cabecalho = MontarPcPedc(pedido, numped);
            var itens = MontarPcPedi(pedido, numped);

            _context.PcPedcs.Add(cabecalho);
            _context.PcPedis.AddRange(itens);

            await _context.SaveChangesAsync(ct);

            pedido.NumpedWinthor = numped;
            pedido.Status = "IMPORTADO";
            pedido.DataImportacaoWinthor = DateTime.Now;
            pedido.DataProcessamento = DateTime.Now;
            pedido.DataAtualizacao = DateTime.Now;
            pedido.MensagemErro = null;

            await _context.SaveChangesAsync(ct);

            await trx.CommitAsync(ct);

            return numped;
        }
        catch (Exception ex)
        {
            await trx.RollbackAsync(ct);

            _context.ChangeTracker.Clear();

            var pedidoErro = await _context.CoteFacilPedidos
                .FirstOrDefaultAsync(x => x.IdPedido == idPedido, ct);

            if (pedidoErro is not null)
            {
                pedidoErro.Status = "ERRO_IMPORTACAO";
                pedidoErro.MensagemErro = CortarTexto(ex.Message, 4000);
                pedidoErro.DataAtualizacao = DateTime.Now;

                await _context.SaveChangesAsync(ct);
            }

            throw;
        }
    }

    private async Task<long> ObterProximoNumpedAsync(CancellationToken ct)
    {
        var connection = _context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();

        command.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
        command.CommandText = "SELECT PCPEDC_DFSEQ.NEXTVAL FROM DUAL";

        var result = await command.ExecuteScalarAsync(ct);

        if (result is null || result == DBNull.Value)
            throw new InvalidOperationException("Não foi possível obter o próximo NUMPED pela sequence PCPEDC_DFSEQ.");

        return Convert.ToInt64(result);
    }

    private static PcPedc MontarPcPedc(CoteFacilPedido pedido, long numped)
    {
        var agora = DateTime.Now;

        var totalPedido = pedido.Itens.Sum(x =>
            x.QuantidadeSolicitada * (x.ValorUnitario ?? 0));

        var totalTabela = pedido.Itens.Sum(x =>
            x.QuantidadeSolicitada * (x.ValorFabrica ?? x.ValorUnitarioNf ?? x.ValorUnitario ?? 0));

        var totalDesconto = totalTabela - totalPedido;

        if (totalDesconto < 0)
            totalDesconto = 0;

        var percentualDesconto = 0m;

        if (totalTabela > 0 && totalDesconto > 0)
            percentualDesconto = Math.Round((totalDesconto / totalTabela) * 100, 6);

        var codFilial = pedido.CodfilialWinthor!;
        var codCli = pedido.CodcliWinthor!.Value;
        var codUsur = pedido.CodusurWinthor!.Value;
        var codPlPag = pedido.CodplpagWinthor!.Value;

        // Ajustar depois com dados reais do cliente/vendedor, se necessário.
        var codPraca = 1;
        var codSupervisor = 1;
        var codCob = string.IsNullOrWhiteSpace(pedido.CodcobWinthor)
            ? "D"
            : pedido.CodcobWinthor;

        var numPedCliente =
            pedido.PedidoCliente ??
            pedido.PedidoCoteFacil?.ToString() ??
            numped.ToString();

        return new PcPedc
        {
            Numped = numped,
            Data = agora,

            Vltotal = totalPedido,
            Vlatend = totalPedido,
            Vltabela = totalTabela,
            Vldesconto = totalDesconto,

            Codcli = codCli,
            Codusur = codUsur,
            Codfilial = codFilial,
            Codfilialnf = codFilial,

            Codpraca = codPraca,
            Codsupervisor = codSupervisor,
            Codplpag = codPlPag,
            Codcob = codCob,

            Numitens = pedido.Itens.Count,
            Posicao = "B",

            Hora = agora.Hour,
            Minuto = agora.Minute,

            Condvenda = 1,
            Perdesc = percentualDesconto,

            Obs = "COTEFACIL",
            Obs1 = CortarTexto($"PED CF: {pedido.PedidoCoteFacil}", 50),
            Obs2 = CortarTexto($"COT CF: {pedido.CotacaoCoteFacil}", 50),

            Numpedcli = CortarTexto(numPedCliente, 15),
            Numpedrca = numped,

            Origemped = "T",
            Exportado = "N",
            Importado = "S",
            Dtimportado = agora,

            Numpedweb = numped,
            NumPedMktPlace = CortarTexto(pedido.PedidoCoteFacil?.ToString() ?? numped.ToString(), 150),
            SistemaLegado = "S",
            RotinaLanc = "DSGESTOR COTEFACIL",

            Vlfrete = 0,
            Vloutrasdesp = 0,
            Totpeso = 0,
            Totvolume = 0
        };
    }
    
    private static List<PcPedi> MontarPcPedi(CoteFacilPedido pedido, long numped)
    {
        var agora = DateTime.Now;

        var codCli = pedido.CodcliWinthor!.Value;
        var codUsur = pedido.CodusurWinthor!.Value;
        var codPlPag = pedido.CodplpagWinthor!.Value;

        var numPedCliente =
            pedido.PedidoCliente ??
            pedido.PedidoCoteFacil?.ToString() ??
            numped.ToString();

        var itens = new List<PcPedi>();

        var sequencia = 0;

        foreach (var item in pedido.Itens.OrderBy(x => x.Sequencia))
        {
            sequencia++;

            var codProd = item.CodprodWinthor!.Value;
            var qt = item.QuantidadeSolicitada;

            var pvenda = item.ValorUnitario ?? item.ValorUnitarioNf ?? item.ValorUnitarioBoleto ?? 0;
            var ptabela = item.ValorFabrica ?? item.ValorUnitarioNf ?? pvenda;

            var perDesc = 0m;

            if (ptabela > 0 && pvenda < ptabela)
                perDesc = Math.Round(((ptabela - pvenda) / ptabela) * 100, 6);

            var codAuxiliar = SomenteNumerosLong(item.Ean);

            var pcPedi = new PcPedi
            {
                Numped = numped,
                Data = agora,

                Codcli = codCli,
                Codprod = codProd,
                Codusur = codUsur,

                Qt = qt,
                Pvenda = pvenda,
                Ptabela = ptabela,

                Posicao = "B",

                St = 0,
                Vlcustofin = 0,
                Vlcustoreal = 0,

                Percom = 0,
                Perdesc = perDesc,

                Qtfalta = 0,
                Numseq = sequencia,

                Codauxiliar = codAuxiliar,

                Pvendabase = ptabela,
                Poriginal = ptabela,
                Qtoriginal = qt,
                Qtorig = qt,

                Vldescfin = item.ValorDescFinanceiro ?? 0,
                Vldesccom = item.ValorDescComercial ?? 0,
                Perdesccom = item.DescontoComercial ?? perDesc,
                Perdescfin = item.DescontoFinanceiro ?? 0,

                Condvenda = 1,
                Codplpag = codPlPag,

                Codfunclanc = 1,
                Rotinalanc = 316,
                Dtlanc = agora,

                Codfuncultalter = 1,
                Rotinaultlalter = 316,
                Dtultlalter = agora,

                Codsupervisor = 1,

                Numpedcli = CortarTexto(numPedCliente, 15),

                Imprime = "S",
                Brinde = "N"
            };

            itens.Add(pcPedi);
        }

        return itens;
    }

    private static void ValidarPedidoAntesImportacao(CoteFacilPedido pedido)
    {
        if (pedido.CodcliWinthor is null or <= 0)
            throw new InvalidOperationException("CODCLI_WINTHOR não informado no pedido.");

        if (string.IsNullOrWhiteSpace(pedido.CodfilialWinthor))
            throw new InvalidOperationException("CODFILIAL_WINTHOR não informado no pedido.");

        if (pedido.CodusurWinthor is null or <= 0)
            throw new InvalidOperationException("CODUSUR_WINTHOR não informado no pedido.");

        if (pedido.CodplpagWinthor is null or <= 0)
            throw new InvalidOperationException("CODPLPAG_WINTHOR não informado no pedido.");

        if (pedido.Itens is null || pedido.Itens.Count == 0)
            throw new InvalidOperationException("Pedido sem itens.");

        foreach (var item in pedido.Itens)
        {
            if (item.CodprodWinthor is null or <= 0)
                throw new InvalidOperationException($"Item {item.Sequencia}: CODPROD_WINTHOR não informado.");

            if (item.QuantidadeSolicitada <= 0)
                throw new InvalidOperationException($"Item {item.Sequencia}: quantidade inválida.");

            var valorUnitario =
                item.ValorUnitario ??
                item.ValorUnitarioNf ??
                item.ValorUnitarioBoleto ??
                0;

            if (valorUnitario <= 0)
                throw new InvalidOperationException($"Item {item.Sequencia}: valor unitário inválido.");
        }
    }

    private static long? SomenteNumerosLong(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return null;

        var numeros = new string(valor.Where(char.IsDigit).ToArray());

        if (long.TryParse(numeros, out var result))
            return result;

        return null;
    }

    private static string? CortarTexto(string? valor, int tamanho)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return valor;

        return valor.Length <= tamanho
            ? valor
            : valor[..tamanho];
    }
}