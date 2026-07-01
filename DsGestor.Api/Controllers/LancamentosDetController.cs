using System.Security.Claims;
using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

public sealed record AtualizarAutorizacaoItemRequest(
    decimal QuantidadeAut,
    decimal PrecoUnitarioAut,
    decimal PercDescontoAut,
    decimal MargemAut
);

[ApiController]
[Route("api/lancamentos/{idLanc:int}/itens")]
[Authorize]
public class LancamentosDetController : ControllerBase
{
    private readonly ILancamentoDetRepository _repo;
    private readonly ILancamentoRepository _lancRepo;
    private readonly IUsuarioRepository _usuarioRepo;

    public LancamentosDetController(
        ILancamentoDetRepository repo,
        ILancamentoRepository lancRepo,
        IUsuarioRepository usuarioRepo)
    {
        _repo = repo;
        _lancRepo = lancRepo;
        _usuarioRepo = usuarioRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetByLancamento(int idLanc)
    {
        var sessao = await GetSessaoAsync();

        var lanc = await _lancRepo.GetByIdAsync(idLanc, incluirItens: false);
        if (lanc is null)
            return NotFound("Lançamento não encontrado.");

        if (!await PodeVisualizarAsync(lanc, sessao))
            return Forbid();

        var itens = (await _repo.GetByLancamentoAsync(idLanc)).ToList();

        if (IsRole(sessao.Perfil, "VENDEDOR"))
        {
            foreach (var it in itens)
            {
                it.PrecoCustoFin = 0;
                it.CodigoIcmTab = 0;
                it.Margem = 0;
                it.MargemIdeal = 0;
                it.MargemAut = 0;
            }
        }

        return Ok(itens);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int idLanc, [FromBody] LancamentoDet det)
    {
        var sessao = await GetSessaoAsync();

        var lanc = await _lancRepo.GetByIdAsync(idLanc, incluirItens: false);
        if (lanc is null)
            return NotFound("Lançamento não encontrado.");

        if ((int)lanc.Status != (int)StatusLancamento.RASCUNHO)
            return BadRequest("Itens só podem ser adicionados em lançamentos RASCUNHO.");

        if (lanc.CodigoUsuario != sessao.CodigoUsuario)
            return Forbid();

        if (det.CodigoProduto <= 0)
            return BadRequest("Informe um produto válido.");

        if (ToDecimal(det.Quantidade) <= 0)
            return BadRequest("A quantidade deve ser maior que zero.");

        if (ToDecimal(det.PrecoUnitario) < 0)
            return BadRequest("O preço unitário não pode ser negativo.");

        if (ToDecimal(det.PercDesconto) < 0 || ToDecimal(det.PercDesconto) > 100)
            return BadRequest("O percentual de desconto deve estar entre 0 e 100.");

        try
        {
            det.Id = 0;
            det.CodigoLancamento = idLanc;

            var created = await _repo.CreateAsync(det);
            return Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int idLanc, int id, [FromBody] LancamentoDet det)
    {
        var sessao = await GetSessaoAsync();

        var lanc = await _lancRepo.GetByIdAsync(idLanc, incluirItens: false);
        if (lanc is null)
            return NotFound("Lançamento não encontrado.");

        if (id <= 0)
            return BadRequest("Item inválido.");

        det.Id = id;
        det.CodigoLancamento = idLanc;

        try
        {
            if ((int)lanc.Status == (int)StatusLancamento.RASCUNHO)
            {
                if (lanc.CodigoUsuario != sessao.CodigoUsuario)
                    return Forbid();

                if (ToDecimal(det.Quantidade) <= 0)
                    return BadRequest("A quantidade deve ser maior que zero.");

                if (ToDecimal(det.PrecoUnitario) < 0)
                    return BadRequest("O preço unitário não pode ser negativo.");

                if (ToDecimal(det.PercDesconto) < 0 || ToDecimal(det.PercDesconto) > 100)
                    return BadRequest("O percentual de desconto deve estar entre 0 e 100.");

                await _repo.UpdateAsync(det);
                return NoContent();
            }

            /*
             * Compatibilidade:
             * Caso algum frontend ainda use PUT /itens/{id} em PENDENTE,
             * convertemos os campos normais recebidos para campos AUT.
             */
            if ((int)lanc.Status == (int)StatusLancamento.PENDENTE)
            {
                if (!IsRole(sessao.Perfil, "ADMIN", "GERENTE"))
                    return Forbid();

                if (ToDecimal(det.Quantidade) <= 0)
                    return BadRequest("A quantidade autorizada deve ser maior que zero.");

                if (ToDecimal(det.PrecoUnitario) < 0)
                    return BadRequest("O preço unitário autorizado não pode ser negativo.");

                if (ToDecimal(det.PercDesconto) < 0 || ToDecimal(det.PercDesconto) > 100)
                    return BadRequest("O percentual de desconto autorizado deve estar entre 0 e 100.");

                var detAut = new LancamentoDet
                {
                    Id = id,
                    CodigoLancamento = idLanc,
                    QuantidadeAut = det.Quantidade,
                    PrecoUnitarioAut = det.PrecoUnitario,
                    PercDescontoAut = det.PercDesconto,
                    MargemAut = det.Margem
                };

                await _repo.UpdateAutorizacaoAsync(detAut, sessao.CodigoUsuario);
                return NoContent();
            }

            return BadRequest("Este lançamento não permite edição de itens no status atual.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}/autorizacao")]
    public async Task<IActionResult> UpdateAutorizacao(
        int idLanc,
        int id,
        [FromBody] AtualizarAutorizacaoItemRequest request)
    {
        var sessao = await GetSessaoAsync();

        var lanc = await _lancRepo.GetByIdAsync(idLanc, incluirItens: false);
        if (lanc is null)
            return NotFound("Lançamento não encontrado.");

        if ((int)lanc.Status != (int)StatusLancamento.PENDENTE)
            return BadRequest("Somente itens de lançamentos PENDENTES podem receber valores autorizados.");

        if (!IsRole(sessao.Perfil, "ADMIN", "GERENTE"))
            return Forbid();

        if (id <= 0)
            return BadRequest("Item inválido.");

        if (request.QuantidadeAut <= 0)
            return BadRequest("A quantidade autorizada deve ser maior que zero.");

        if (request.PrecoUnitarioAut < 0)
            return BadRequest("O preço unitário autorizado não pode ser negativo.");

        if (request.PercDescontoAut < 0 || request.PercDescontoAut > 100)
            return BadRequest("O percentual de desconto autorizado deve estar entre 0 e 100.");

        try
        {
            /*
             * Importante:
             * Aqui preenchemos os campos AUT de verdade.
             * Não preencher Quantidade/PrecoUnitario/PercDesconto,
             * senão o repository recebe Aut como zero/null.
             */
            var det = new LancamentoDet
            {
                Id = id,
                CodigoLancamento = idLanc,
                QuantidadeAut = request.QuantidadeAut,
                PrecoUnitarioAut = request.PrecoUnitarioAut,
                PercDescontoAut = request.PercDescontoAut,
                MargemAut = request.MargemAut
            };

            await _repo.UpdateAutorizacaoAsync(det, sessao.CodigoUsuario);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int idLanc, int id)
    {
        var sessao = await GetSessaoAsync();

        var lanc = await _lancRepo.GetByIdAsync(idLanc, incluirItens: false);
        if (lanc is null)
            return NotFound("Lançamento não encontrado.");

        if (id <= 0)
            return BadRequest("Item inválido.");

        var isRascunho = (int)lanc.Status == (int)StatusLancamento.RASCUNHO;
        var isPendente = (int)lanc.Status == (int)StatusLancamento.PENDENTE;

        if (isRascunho)
        {
            if (lanc.CodigoUsuario != sessao.CodigoUsuario)
                return Forbid();
        }
        else if (isPendente)
        {
            if (!IsRole(sessao.Perfil, "ADMIN", "GERENTE"))
                return Forbid();
        }
        else
        {
            return BadRequest("Itens só podem ser removidos em lançamentos RASCUNHO ou PENDENTE.");
        }

        try
        {
            await _repo.DeleteAsync(id, idLanc);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task<bool> PodeVisualizarAsync(Lancamento lanc, SessaoUsuario sessao)
    {
        var status = (int)lanc.Status;

        if (IsRole(sessao.Perfil, "ADMIN", "GERENTE"))
        {
            return status != (int)StatusLancamento.RASCUNHO ||
                   lanc.CodigoUsuario == sessao.CodigoUsuario;
        }

        if (IsRole(sessao.Perfil, "VENDEDOR"))
        {
            var codigoVendedor = sessao.CodigoVendedor ?? 0;

            if (status == (int)StatusLancamento.RASCUNHO)
            {
                return lanc.CodigoUsuario == sessao.CodigoUsuario &&
                       lanc.CodigoVendedor == codigoVendedor;
            }

            return lanc.CodigoVendedor == codigoVendedor;
        }

        if (IsRole(sessao.Perfil, "SUPERVISOR"))
        {
            if (status == (int)StatusLancamento.RASCUNHO)
                return lanc.CodigoUsuario == sessao.CodigoUsuario;

            var codigoSupervisor = sessao.CodigoSupervisor;

            if (!codigoSupervisor.HasValue)
                return lanc.CodigoUsuario == sessao.CodigoUsuario;

            var usuarios = await _usuarioRepo.GetAllAsync();

            var vendedores = usuarios
                .Where(u =>
                    u.CodigoVendedor.HasValue &&
                    u.CodigoSupervisor.HasValue &&
                    u.CodigoSupervisor.Value == codigoSupervisor.Value &&
                    string.Equals(
                        u.Perfil.ToString(),
                        "Vendedor",
                        StringComparison.OrdinalIgnoreCase))
                .Select(u => u.CodigoVendedor!.Value)
                .ToHashSet();

            return vendedores.Contains(lanc.CodigoVendedor) ||
                   lanc.CodigoUsuario == sessao.CodigoUsuario;
        }

        return status != (int)StatusLancamento.RASCUNHO ||
               lanc.CodigoUsuario == sessao.CodigoUsuario;
    }

    private async Task<SessaoUsuario> GetSessaoAsync()
    {
        var codigoUsuario =
            GetIntClaim(ClaimTypes.NameIdentifier) ??
            GetIntClaim("sub") ??
            0;

        var perfil =
            User.FindFirstValue(ClaimTypes.Role) ??
            User.FindFirstValue("role") ??
            string.Empty;

        var codigoVendedor = GetIntClaim("codigoVendedor");
        var codigoSupervisor = GetIntClaim("codigoSupervisor");

        var usuario = codigoUsuario > 0
            ? await _usuarioRepo.GetByIdAsync(codigoUsuario)
            : null;

        codigoVendedor ??= usuario?.CodigoVendedor;
        codigoSupervisor ??= usuario?.CodigoSupervisor;

        return new SessaoUsuario(
            codigoUsuario,
            perfil,
            codigoVendedor,
            codigoSupervisor
        );
    }

    private int? GetIntClaim(string claimType)
    {
        var value = User.FindFirst(claimType)?.Value;
        return int.TryParse(value, out var numero) ? numero : null;
    }

    private static bool IsRole(string? perfil, params string[] roles)
    {
        if (string.IsNullOrWhiteSpace(perfil))
            return false;

        return roles.Any(role =>
            string.Equals(perfil, role, StringComparison.OrdinalIgnoreCase));
    }

    private static decimal ToDecimal(decimal? value)
    {
        return value ?? 0m;
    }

    private sealed record SessaoUsuario(
        int CodigoUsuario,
        string Perfil,
        int? CodigoVendedor,
        int? CodigoSupervisor
    );
}