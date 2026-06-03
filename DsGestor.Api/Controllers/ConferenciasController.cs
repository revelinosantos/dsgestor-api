using DsGestor.Application.DTOs.Conferencias;
using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

[ApiController]
[Route("api/conferencias")]
[Authorize]
public class ConferenciasController : ControllerBase
{
    private readonly IConferenciaRepository _repository;

    public ConferenciasController(IConferenciaRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("{numped:decimal}/iniciar")]
    public async Task<IActionResult> Iniciar(
        decimal numped,
        [FromQuery] int numnota = 0,
        [FromQuery] int codUsuario = 0)
    {
        try
        {
            var existe = await _repository.ExisteConferenciaAsync(numped, numnota);

            if (!existe)
            {
                await _repository.ImportarPedidoAsync(numped, numnota, codUsuario);
            }

            var itens = await _repository.ObterItensAsync(numped, numnota);

            if (!itens.Any())
                return NotFound(new { mensagem = $"Pedido {numped} não encontrado para conferência." });

            PrepararInicioConferencia(itens, codUsuario);

            await _repository.AtualizarItensAsync(itens);
            await _repository.SalvarAsync();

            var resumo = MontarResumo(numped, numnota, itens);

            return Ok(resumo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{numped:decimal}")]
    public async Task<IActionResult> ObterResumo(
        decimal numped,
        [FromQuery] int numnota = 0)
    {
        try
        {
            var itens = await _repository.ObterItensAsync(numped, numnota);

            if (!itens.Any())
                return NotFound(new { mensagem = $"Pedido {numped} não encontrado para conferência." });

            var resumo = MontarResumo(numped, numnota, itens);

            return Ok(resumo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{numped:decimal}/itens")]
    public async Task<IActionResult> ObterItens(
        decimal numped,
        [FromQuery] int numnota = 0)
    {
        try
        {
            var itens = await _repository.ObterItensAsync(numped, numnota);

            if (!itens.Any())
                return NotFound(new { mensagem = $"Pedido {numped} não encontrado para conferência." });

            var result = itens
                .OrderBy(x => x.Numseq)
                .Select(MapItem)
                .ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{numped:decimal}/produto/{ean}")]
    public async Task<IActionResult> BuscarProduto(
        decimal numped,
        string ean,
        [FromQuery] int numnota = 0)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ean))
                return BadRequest(new { mensagem = "EAN/código auxiliar é obrigatório." });

            var item = await _repository.BuscarItemPorEanAsync(numped, numnota, ean);

            if (item == null)
                return NotFound(new { mensagem = $"Produto/EAN {ean} não encontrado no pedido {numped}." });

            return Ok(MapItem(item));
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("{numped:decimal}/itens/conferir")]
    public async Task<IActionResult> ConferirItem(
        decimal numped,
        [FromBody] ConferirItemRequest request,
        [FromQuery] int numnota = 0)
    {
        try
        {
            if (request == null)
                return BadRequest(new { mensagem = "Request inválido." });

            if (string.IsNullOrWhiteSpace(request.Ean))
                return BadRequest(new { mensagem = "EAN/código auxiliar é obrigatório." });

            if (request.Quantidade <= 0)
                return BadRequest(new { mensagem = "Quantidade deve ser maior que zero." });

            var item = await _repository.BuscarItemPorEanAsync(numped, numnota, request.Ean);

            if (item == null)
                return NotFound(new { mensagem = $"Produto/EAN {request.Ean} não encontrado no pedido {numped}." });

            var qtOriginal = ObterQuantidadeOriginal(item);
            var qtConferidaAtual = item.Qtconfunid ?? 0;
            var novaQtConferida = qtConferidaAtual + request.Quantidade;

            item.Qtconfunid = novaQtConferida;
            item.Qtconf = novaQtConferida;
            item.Qtfalta = Math.Max(qtOriginal - novaQtConferida, 0);
            item.Conferidoitem = novaQtConferida >= qtOriginal ? "S" : "N";
            item.Dataultleitura = DateTime.Now;
            item.Android = "S";

            await _repository.AtualizarAsync(item);

            var itensPedido = await _repository.ObterItensAsync(numped, numnota);

            AtualizarStatusGeralDaConferencia(itensPedido);

            await _repository.AtualizarItensAsync(itensPedido);
            await _repository.SalvarAsync();

            var itemAtualizado = await _repository.ObterItemPorIdAsync(item.Confid);

            return Ok(MapItem(itemAtualizado ?? item));
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{numped:decimal}/validar")]
    public async Task<IActionResult> Validar(
        decimal numped,
        [FromQuery] int numnota = 0)
    {
        try
        {
            var itens = await _repository.ObterItensAsync(numped, numnota);

            if (!itens.Any())
                return NotFound(new { mensagem = $"Pedido {numped} não encontrado para conferência." });

            RecalcularItens(itens);

            var completa = ConferenciaEstaCompleta(itens);

            foreach (var item in itens)
            {
                item.Conferido = completa ? "S" : "N";
                item.Statusconf = completa ? "VALIDADA" : "EM_CONFERENCIA";
            }

            await _repository.AtualizarItensAsync(itens);
            await _repository.SalvarAsync();

            var validacao = MontarValidacao(numped, numnota, itens);

            return Ok(validacao);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("{numped:decimal}/finalizar")]
    public async Task<IActionResult> Finalizar(
        decimal numped,
        [FromBody] FinalizarConferenciaRequest request,
        [FromQuery] int numnota = 0)
    {
        try
        {
            if (request == null)
                return BadRequest(new { mensagem = "Request inválido." });

            if (request.CodUsuario <= 0)
                return BadRequest(new { mensagem = "Código do usuário é obrigatório." });

            var itens = await _repository.ObterItensAsync(numped, numnota);

            if (!itens.Any())
                return NotFound(new { mensagem = $"Pedido {numped} não encontrado para conferência." });

            if (itens.Any(x => (x.Numtransvenda ?? 0) > 0))
            {
                return BadRequest(new
                {
                    mensagem = "Este pedido já possui NUMTRANSVENDA. A finalização após emissão da nota ainda não foi implementada neste endpoint."
                });
            }

            RecalcularItens(itens);

            if (!ConferenciaEstaCompleta(itens))
            {
                var validacao = MontarValidacao(numped, numnota, itens);

                return BadRequest(new
                {
                    mensagem = "A conferência possui itens pendentes. Use a finalização com divergência mediante supervisor.",
                    validacao
                });
            }

            await _repository.FinalizarPedidoAsync(
                numped,
                numnota,
                request.CodUsuario,
                finalizarComDivergencia: false
            );

            var itensAtualizados = await _repository.ObterItensAsync(numped, numnota);
            var resumo = MontarResumo(numped, numnota, itensAtualizados);

            return Ok(resumo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("{numped:decimal}/finalizar-divergencia")]
    public async Task<IActionResult> FinalizarComDivergencia(
        decimal numped,
        [FromBody] FinalizarDivergenciaRequest request,
        [FromQuery] int numnota = 0)
    {
        try
        {
            if (request == null)
                return BadRequest(new { mensagem = "Request inválido." });

            if (request.CodUsuario <= 0)
                return BadRequest(new { mensagem = "Código do usuário é obrigatório." });

            if (request.CodSupervisor <= 0)
                return BadRequest(new { mensagem = "Código do supervisor é obrigatório." });

            if (string.IsNullOrWhiteSpace(request.SenhaSupervisor))
                return BadRequest(new { mensagem = "Senha do supervisor é obrigatória." });

            var itens = await _repository.ObterItensAsync(numped, numnota);

            if (!itens.Any())
                return NotFound(new { mensagem = $"Pedido {numped} não encontrado para conferência." });

            if (itens.Any(x => (x.Numtransvenda ?? 0) > 0))
            {
                return BadRequest(new
                {
                    mensagem = "Este pedido já possui NUMTRANSVENDA. A finalização com divergência após emissão da nota ainda não foi implementada neste endpoint."
                });
            }

            var supervisorValido = await ValidarSupervisorAsync(request.CodSupervisor, request.SenhaSupervisor);

            if (!supervisorValido)
                return Unauthorized(new { mensagem = "Supervisor ou senha inválidos." });

            RecalcularItens(itens);

            await _repository.FinalizarPedidoAsync(
                numped,
                numnota,
                request.CodUsuario,
                finalizarComDivergencia: true,
                codSupervisor: request.CodSupervisor,
                observacao: request.Observacao
            );

            var itensAtualizados = await _repository.ObterItensAsync(numped, numnota);
            var resumo = MontarResumo(numped, numnota, itensAtualizados);

            return Ok(resumo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    private static void PrepararInicioConferencia(List<Conferencia> itens, int codUsuario)
    {
        var agora = DateTime.Now;

        foreach (var item in itens)
        {
            item.Datainiconf ??= agora;

            if (codUsuario > 0)
                item.Usuarioinicio ??= codUsuario;

            if (string.IsNullOrWhiteSpace(item.Statusconf))
                item.Statusconf = "ABERTA";

            if (string.IsNullOrWhiteSpace(item.Conferido))
                item.Conferido = "N";

            if (string.IsNullOrWhiteSpace(item.Conferidoitem))
                item.Conferidoitem = "N";

            item.Qtconfunid ??= 0;
            item.Qtconf ??= 0;

            var qtOriginal = ObterQuantidadeOriginal(item);
            item.Qtfalta = Math.Max(qtOriginal - (item.Qtconfunid ?? 0), 0);
        }
    }

    private static void RecalcularItens(List<Conferencia> itens)
    {
        foreach (var item in itens)
        {
            var qtOriginal = ObterQuantidadeOriginal(item);
            var qtConf = item.Qtconfunid ?? 0;

            item.Qtfalta = Math.Max(qtOriginal - qtConf, 0);
            item.Conferidoitem = qtConf >= qtOriginal ? "S" : "N";
        }
    }

    private static void AtualizarStatusGeralDaConferencia(List<Conferencia> itens)
    {
        RecalcularItens(itens);

        var completa = ConferenciaEstaCompleta(itens);

        foreach (var item in itens)
        {
            item.Conferido = completa ? "S" : "N";
            item.Statusconf = completa ? "VALIDADA" : "EM_CONFERENCIA";
        }
    }

    private static bool ConferenciaEstaCompleta(List<Conferencia> itens)
    {
        return itens.All(x =>
        {
            var qtOriginal = ObterQuantidadeOriginal(x);
            var qtConf = x.Qtconfunid ?? 0;

            return qtConf >= qtOriginal;
        });
    }

    private static int ObterQuantidadeOriginal(Conferencia item)
    {
        return item.Qt ?? item.Qtoriginal ?? 0;
    }

    private static ConferenciaItemDto MapItem(Conferencia item)
    {
        var qtOriginal = ObterQuantidadeOriginal(item);
        var qtConferida = item.Qtconfunid ?? 0;
        var qtFalta = Math.Max(qtOriginal - qtConferida, 0);

        return new ConferenciaItemDto
        {
            Confid = item.Confid,
            Numped = item.Numped,
            Numnota = item.Numnota,

            Codprod = item.Codprod,
            Numseq = item.Numseq,
            Descricao = item.Descricao,

            Codauxiliar = item.Codauxiliar,
            Codauxiliar2 = item.Codauxiliar2,
            Lpvcod = item.Lpvcod,
            Lpvcodfind = item.Lpvcodfind,

            QtOriginal = qtOriginal,
            QtConferida = qtConferida,
            QtFalta = qtFalta,

            Pvenda = item.Pvenda,
            PesoBrutoProduto = item.Pesobrutopro,
            PesoLiquidoProduto = item.Pesoliqpro,

            ConferidoItem = item.Conferidoitem
        };
    }

    private static ConferenciaResumoDto MontarResumo(
        decimal numped,
        int numnota,
        List<Conferencia> itens)
    {
        var primeiro = itens.First();

        RecalcularItens(itens);

        return new ConferenciaResumoDto
        {
            Numped = numped,
            Numnota = numnota,
            Codfilial = primeiro.Codfilial,
            Codcli = primeiro.Codcli,
            Cliente = primeiro.Cliente,
            Vltotal = primeiro.Vltotal,
            Statusconf = primeiro.Statusconf,

            TotalItens = itens.Count,
            TotalItensConferidos = itens.Count(x => x.Conferidoitem == "S"),
            TotalItensPendentes = itens.Count(x => x.Conferidoitem != "S"),

            TotalUnidades = itens.Sum(ObterQuantidadeOriginal),
            TotalUnidadesConferidas = itens.Sum(x => x.Qtconfunid ?? 0),
            TotalUnidadesPendentes = itens.Sum(x => x.Qtfalta ?? 0),

            PesoBrutoTotal = itens.Sum(x => (x.Pesobrutopro ?? 0) * (x.Qtconfunid ?? 0)),
            PesoLiquidoTotal = itens.Sum(x => (x.Pesoliqpro ?? 0) * (x.Qtconfunid ?? 0)),

            DataInicioConf = itens.Min(x => x.Datainiconf),
            DataFimConf = itens.Max(x => x.Datafimconf)
        };
    }

    private static ValidacaoConferenciaDto MontarValidacao(
        decimal numped,
        int numnota,
        List<Conferencia> itens)
    {
        RecalcularItens(itens);

        var faltantes = itens
            .Where(x => (x.Qtfalta ?? 0) > 0)
            .OrderBy(x => x.Numseq)
            .Select(MapItem)
            .ToList();

        return new ValidacaoConferenciaDto
        {
            Numped = numped,
            Numnota = numnota,

            ConferenciaCompleta = !faltantes.Any(),

            TotalItens = itens.Count,
            ItensConferidos = itens.Count(x => x.Conferidoitem == "S"),
            ItensPendentes = itens.Count(x => x.Conferidoitem != "S"),

            TotalUnidades = itens.Sum(ObterQuantidadeOriginal),
            TotalUnidadesConferidas = itens.Sum(x => x.Qtconfunid ?? 0),
            TotalUnidadesPendentes = itens.Sum(x => x.Qtfalta ?? 0),

            Faltantes = faltantes
        };
    }

    private async Task<bool> ValidarSupervisorAsync(int codSupervisor, string senhaSupervisor)
    {
        /*
            Próximo ajuste real:
            validar contra DSGESTOR_USUARIO.

            Como sugestão, depois podemos mover isso para repository:
            - buscar usuário pelo código/supervisor;
            - validar senha usando o mesmo padrão do login atual;
            - conferir perfil Admin/Gerente/Supervisor.
        */

        await Task.CompletedTask;

        return true;
    }
}