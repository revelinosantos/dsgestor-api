using System.Security.Claims;
using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

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
        if (lanc is null) return NotFound();

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
        if (lanc is null) return NotFound();

        // Só rascunho do próprio usuário
        if ((int)lanc.Status != (int)StatusLancamento.RASCUNHO)
            return BadRequest("Itens só podem ser adicionados em RASCUNHO.");

        if (lanc.CodigoUsuario != sessao.CodigoUsuario)
            return Forbid();

        try
        {
            det.CodigoLancamento = idLanc;
            var created = await _repo.CreateAsync(det);
            return Ok(created);
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
        if (lanc is null) return NotFound();

        det.Id = id;
        det.CodigoLancamento = idLanc;

        try
        {
            // 1) RASCUNHO -> atualização normal (somente dono)
            if ((int)lanc.Status == (int)StatusLancamento.RASCUNHO)
            {
                if (lanc.CodigoUsuario != sessao.CodigoUsuario)
                    return Forbid();

                await _repo.UpdateAsync(det);
                return NoContent();
            }

            // 2) PENDENTE -> Admin/Gerente altera campos AUT (mantém solicitado original)
            if ((int)lanc.Status == (int)StatusLancamento.PENDENTE)
            {
                if (!IsRole(sessao.Perfil, "ADMIN", "GERENTE"))
                    return Forbid();

                await _repo.UpdateAutorizacaoAsync(det, sessao.CodigoUsuario);
                return NoContent();
            }

            return BadRequest("Este lançamento não permite edição de itens no status atual.");
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
        if (lanc is null) return NotFound();

        // Remover item somente no rascunho do próprio usuário
        if ((int)lanc.Status != (int)StatusLancamento.RASCUNHO)
            return BadRequest("Só é permitido remover item em RASCUNHO.");

        if (lanc.CodigoUsuario != sessao.CodigoUsuario)
            return Forbid();

        try
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ============================
    // Reuso de regra de visibilidade (versão compacta)
    // ============================
    private async Task<bool> PodeVisualizarAsync(Lancamento l, SessaoUsuario sessao)
    {
        var status = (int)l.Status;

        // ADMIN / GERENTE
        if (IsRole(sessao.Perfil, "ADMIN", "GERENTE"))
            return status != (int)StatusLancamento.RASCUNHO || l.CodigoUsuario == sessao.CodigoUsuario;

        // VENDEDOR
        if (IsRole(sessao.Perfil, "VENDEDOR"))
        {
            var codVend = sessao.CodigoVendedor ?? 0;
            if (status == (int)StatusLancamento.RASCUNHO)
                return l.CodigoUsuario == sessao.CodigoUsuario && l.CodigoVendedor == codVend;

            return l.CodigoVendedor == codVend;
        }

        // SUPERVISOR
        if (IsRole(sessao.Perfil, "SUPERVISOR"))
        {
            if (status == (int)StatusLancamento.RASCUNHO)
                return l.CodigoUsuario == sessao.CodigoUsuario;

            var codSup = sessao.CodigoSupervisor;
            if (!codSup.HasValue) return l.CodigoUsuario == sessao.CodigoUsuario;

            var usuarios = await _usuarioRepo.GetAllAsync();
            var vendors = usuarios
                .Where(u =>
                    u.CodigoVendedor.HasValue
                    && u.CodigoSupervisor.HasValue
                    && u.CodigoSupervisor.Value == codSup.Value
                    && string.Equals(u.Perfil.ToString(), "Vendedor", StringComparison.OrdinalIgnoreCase))
                .Select(u => u.CodigoVendedor!.Value)
                .ToHashSet();

            return vendors.Contains(l.CodigoVendedor) || l.CodigoUsuario == sessao.CodigoUsuario;
        }

        // Outros perfis
        return status != (int)StatusLancamento.RASCUNHO || l.CodigoUsuario == sessao.CodigoUsuario;
    }

    // ============================
    // Sessão / claims
    // ============================
    private async Task<SessaoUsuario> GetSessaoAsync()
    {
        var id = GetIntClaim(ClaimTypes.NameIdentifier)
                 ?? GetIntClaim("sub")
                 ?? 0;

        var perfil = User.FindFirstValue(ClaimTypes.Role)
                    ?? User.FindFirstValue("role")
                    ?? "";

        var codVend = GetIntClaim("codigoVendedor");
        var codSup = GetIntClaim("codigoSupervisor");

        var usuario = id > 0 ? await _usuarioRepo.GetByIdAsync(id) : null;
        codVend ??= usuario?.CodigoVendedor;
        codSup ??= usuario?.CodigoSupervisor;

        return new SessaoUsuario(id, perfil, codVend, codSup);
    }

    private int? GetIntClaim(string claimType)
    {
        var value = User.FindFirst(claimType)?.Value;
        if (int.TryParse(value, out var n)) return n;
        return null;
    }

    private static bool IsRole(string? perfil, params string[] roles)
    {
        if (string.IsNullOrWhiteSpace(perfil)) return false;
        return roles.Any(r => string.Equals(perfil, r, StringComparison.OrdinalIgnoreCase));
    }

    private sealed record SessaoUsuario(int CodigoUsuario, string Perfil, int? CodigoVendedor, int? CodigoSupervisor);
}