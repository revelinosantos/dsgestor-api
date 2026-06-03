using System.Security.Claims;
using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

public record DraftRequest(
    DateTime Data,
    string CodigoFilial,
    int NumRegiao,
    int CodigoVendedor,
    int CodigoCliente,
    int CodigoUsuario, // será ignorado e substituído pelo usuário logado
    string? Observacao
);

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LancamentosController : ControllerBase
{
    private readonly ILancamentoRepository _repo;
    private readonly IUsuarioRepository _usuarioRepo;

    public LancamentosController(ILancamentoRepository repo, IUsuarioRepository usuarioRepo)
    {
        _repo = repo;
        _usuarioRepo = usuarioRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sessao = await GetSessaoAsync();
        var all = (await _repo.GetAllAsync()).ToList();

        var filtered = await FiltrarPorPerfilAsync(all, sessao);

        if (IsRole(sessao.Perfil, "VENDEDOR"))
            filtered.ForEach(MascararCamposSensiveisVendedor);

        return Ok(filtered);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] bool incluirItens = false)
    {
        var sessao = await GetSessaoAsync();

        var item = await _repo.GetByIdAsync(id, incluirItens);
        if (item is null) return NotFound();

        if (!await PodeVisualizarAsync(item, sessao))
            return Forbid();

        if (IsRole(sessao.Perfil, "VENDEDOR"))
            MascararCamposSensiveisVendedor(item);

        return Ok(item);
    }

    [HttpGet("por-usuario/{codigoUsuario:int}")]
    public async Task<IActionResult> GetByUsuario(int codigoUsuario)
    {
        var sessao = await GetSessaoAsync();
        var all = (await _repo.GetAllAsync()).ToList();

        var filtered = await FiltrarPorPerfilAsync(all, sessao);
        filtered = filtered.Where(x => x.CodigoUsuario == codigoUsuario).ToList();

        return Ok(filtered);
    }

    [HttpGet("por-status/{status:int}")]
    public async Task<IActionResult> GetByStatus(int status)
    {
        var sessao = await GetSessaoAsync();
        var all = (await _repo.GetAllAsync()).ToList();

        var filtered = await FiltrarPorPerfilAsync(all, sessao);
        filtered = filtered.Where(x => (int)x.Status == status).ToList();

        return Ok(filtered);
    }

    [HttpGet("draft")]
    public async Task<IActionResult> GetDraft(
        [FromQuery] string codigoFilial,
        [FromQuery] int numRegiao,
        [FromQuery] int codigoCliente,
        [FromQuery] int codigoVendedor,
        [FromQuery] int codigoUsuario, // ignorado
        [FromQuery] bool incluirItens = true)
    {
        var sessao = await GetSessaoAsync();

        var draft = await _repo.GetDraftAsync(codigoFilial, numRegiao, codigoCliente, codigoVendedor, sessao.CodigoUsuario, incluirItens);
        if (draft is null) return NotFound();

        // rascunho: só o dono
        if (draft.CodigoUsuario != sessao.CodigoUsuario) return Forbid();

        if (IsRole(sessao.Perfil, "VENDEDOR"))
            MascararCamposSensiveisVendedor(draft);

        return Ok(draft);
    }

    [HttpPost("draft")]
    public async Task<IActionResult> CreateOrGetDraft([FromBody] DraftRequest req)
    {
        var sessao = await GetSessaoAsync();

        var draft = new Lancamento
        {
            Data = req.Data,
            CodigoFilial = req.CodigoFilial,
            NumRegiao = req.NumRegiao,
            CodigoVendedor = req.CodigoVendedor,
            CodigoCliente = req.CodigoCliente,
            CodigoUsuario = sessao.CodigoUsuario, // ✅ garante usuário logado
            Observacao = string.IsNullOrWhiteSpace(req.Observacao) ? null : req.Observacao.Trim(),
            Status = StatusLancamento.RASCUNHO
        };

        var created = await _repo.CreateOrGetDraftAsync(draft);

        if (IsRole(sessao.Perfil, "VENDEDOR"))
            MascararCamposSensiveisVendedor(created);

        return Ok(created);
    }

    [HttpPost("{id:int}/finalizar")]
    public async Task<IActionResult> Finalizar(int id)
    {
        var sessao = await GetSessaoAsync();

        var lanc = await _repo.GetByIdAsync(id, incluirItens: true);
        if (lanc is null) return NotFound();

        if ((int)lanc.Status != (int)StatusLancamento.RASCUNHO)
            return BadRequest("Somente rascunho pode ser finalizado.");

        if (lanc.CodigoUsuario != sessao.CodigoUsuario)
            return Forbid();

        try
        {
            var ok = await _repo.FinalizarAsync(id);
            return ok ? NoContent() : BadRequest("Não foi possível finalizar (verifique se está em RASCUNHO).");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Lancamento lanc)
    {
        // Mantém endpoint antigo, mas idealmente usar o draft
        var created = await _repo.CreateAsync(lanc);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] DraftRequest req)
    {
        var sessao = await GetSessaoAsync();

        var atual = await _repo.GetByIdAsync(id, incluirItens: false);
        if (atual is null) return NotFound();

        var isAdminGerente = IsRole(sessao.Perfil, "ADMIN", "GERENTE");
        var isDono = atual.CodigoUsuario == sessao.CodigoUsuario;

        // RASCUNHO:
        // - dono pode atualizar cabeçalho
        // - observação só admin/gerente altera
        if ((int)atual.Status == (int)StatusLancamento.RASCUNHO)
        {
            if (!isDono) return Forbid();

            atual.Data = req.Data;
            atual.CodigoFilial = req.CodigoFilial;
            atual.NumRegiao = req.NumRegiao;
            atual.CodigoVendedor = req.CodigoVendedor;
            atual.CodigoCliente = req.CodigoCliente;

            if (isAdminGerente)
                atual.Observacao = string.IsNullOrWhiteSpace(req.Observacao) ? null : req.Observacao.Trim();
        }
        // PENDENTE:
        // - somente admin/gerente
        // - aqui deixamos alterar só observação
        else if ((int)atual.Status == (int)StatusLancamento.PENDENTE)
        {
            if (!isAdminGerente) return Forbid();

            atual.Observacao = string.IsNullOrWhiteSpace(req.Observacao) ? null : req.Observacao.Trim();
        }
        else
        {
            return BadRequest("Somente lançamentos em RASCUNHO ou PENDENTE podem ser alterados.");
        }

        try
        {
            await _repo.UpdateAsync(atual);

            if (IsRole(sessao.Perfil, "VENDEDOR"))
                MascararCamposSensiveisVendedor(atual);

            return Ok(atual);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("{id:int}/autorizar")]
    [Authorize(Roles = "ADMIN,GERENTE")]
    public async Task<IActionResult> Autorizar(int id, CancellationToken ct)
    {
        var sessao = await GetSessaoAsync();

        var lanc = await _repo.GetByIdAsync(id, incluirItens: false);
        if (lanc is null) return NotFound();

        if (!await PodeVisualizarAsync(lanc, sessao))
            return Forbid();

        try
        {
            var codigoPromocao = await _repo.AutorizarAsync(id, sessao.CodigoUsuario, ct);

            return Ok(new
            {
                sucesso = true,
                Id = id,
                CodigoPromocao = codigoPromocao,
                status = "AUTORIZADO"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sessao = await GetSessaoAsync();

        var lanc = await _repo.GetByIdAsync(id, incluirItens: false);
        if (lanc is null) return NotFound();

        var status = (int)lanc.Status;

        // rascunho -> só dono
        if (status == (int)StatusLancamento.RASCUNHO)
        {
            if (lanc.CodigoUsuario != sessao.CodigoUsuario) return Forbid();
        }
        // pendente -> dono ou admin/gerente
        else if (status == (int)StatusLancamento.PENDENTE)
        {
            var isAdminGerente = IsRole(sessao.Perfil, "ADMIN", "GERENTE");
            if (!isAdminGerente && lanc.CodigoUsuario != sessao.CodigoUsuario)
                return Forbid();
        }
        else
        {
            return BadRequest("Somente lançamentos em RASCUNHO ou PENDENTE podem ser excluídos.");
        }

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
    // Regras de VISIBILIDADE
    // ============================
    private async Task<List<Lancamento>> FiltrarPorPerfilAsync(List<Lancamento> all, SessaoUsuario sessao)
    {
        // ADMIN / GERENTE / outros:
        // - veem tudo
        // - mas rascunho: só os próprios
        if (IsRole(sessao.Perfil, "ADMIN", "GERENTE"))
        {
            return all.Where(l =>
                (int)l.Status != (int)StatusLancamento.RASCUNHO
                || l.CodigoUsuario == sessao.CodigoUsuario
            ).ToList();
        }

        // VENDEDOR:
        // - apenas solicitações do mesmo vendedor
        // - rascunho: somente o próprio usuário
        if (IsRole(sessao.Perfil, "VENDEDOR"))
        {
            var codVend = sessao.CodigoVendedor ?? 0;

            return all.Where(l =>
            {
                var isRascunho = (int)l.Status == (int)StatusLancamento.RASCUNHO;
                if (isRascunho)
                    return l.CodigoUsuario == sessao.CodigoUsuario && l.CodigoVendedor == codVend;

                return l.CodigoVendedor == codVend;
            }).ToList();
        }

        // SUPERVISOR:
        // - vê pendentes/finalizados dos vendedores vinculados
        // - NÃO vê rascunho dos vendedores
        // - vê seus próprios rascunhos
        if (IsRole(sessao.Perfil, "SUPERVISOR"))
        {
            var codSup = sessao.CodigoSupervisor;
            var vendors = new HashSet<int>();

            if (codSup.HasValue)
            {
                var usuarios = await _usuarioRepo.GetAllAsync();
                vendors = usuarios
                    .Where(u =>
                        u.CodigoVendedor.HasValue
                        && u.CodigoSupervisor.HasValue
                        && u.CodigoSupervisor.Value == codSup.Value
                        && string.Equals(u.Perfil.ToString(), "Vendedor", StringComparison.OrdinalIgnoreCase))
                    .Select(u => u.CodigoVendedor!.Value)
                    .ToHashSet();
            }

            return all.Where(l =>
            {
                var isRascunho = (int)l.Status == (int)StatusLancamento.RASCUNHO;

                if (isRascunho)
                {
                    // só rascunhos do próprio supervisor
                    return l.CodigoUsuario == sessao.CodigoUsuario;
                }

                // pendente/autorizado/rejeitado dos vendedores vinculados OU do próprio supervisor
                return vendors.Contains(l.CodigoVendedor) || l.CodigoUsuario == sessao.CodigoUsuario;
            }).ToList();
        }

        // Outros perfis:
        // - todos os lançamentos
        // - rascunho só do próprio
        return all.Where(l =>
            (int)l.Status != (int)StatusLancamento.RASCUNHO
            || l.CodigoUsuario == sessao.CodigoUsuario
        ).ToList();
    }

    private async Task<bool> PodeVisualizarAsync(Lancamento l, SessaoUsuario sessao)
    {
        var list = await FiltrarPorPerfilAsync(new List<Lancamento> { l }, sessao);
        return list.Any();
    }

    private void MascararCamposSensiveisVendedor(Lancamento lanc)
    {
        if (lanc.Itens == null) return;

        foreach (var it in lanc.Itens)
        {
            it.PrecoCustoFin = 0;
            it.CodigoIcmTab = 0;
            it.Margem = 0;
            it.MargemIdeal = 0;
            it.MargemAut = 0;
        }
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

        // fallback pelo cadastro
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