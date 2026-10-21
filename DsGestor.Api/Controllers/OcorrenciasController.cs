using System.Security.Claims;
using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

public sealed record CriarOcorrenciaRequest(
    decimal NumeroNota,
    string SerieNota,
    string CodigoFilial,
    string? CodigoFilialNf,
    decimal NumeroTransacaoVenda,
    int CodigoCliente,
    int? CodigoVendedor,
    int? CodigoMotoristaNota,
    int CodigoMotorista,
    bool DevolucaoTotal,
    string? Observacao);

public sealed record CriarOcorrenciaComItensRequest(
    decimal NumeroNota,
    string SerieNota,
    string CodigoFilial,
    string? CodigoFilialNf,
    decimal NumeroTransacaoVenda,
    int CodigoCliente,
    int? CodigoVendedor,
    int? CodigoMotoristaNota,
    int CodigoMotorista,
    bool DevolucaoTotal,
    string? Observacao,
    IReadOnlyCollection<CriarOcorrenciaItemRequest> Itens);

public sealed record AtualizarOcorrenciaRequest(
    int CodigoMotorista,
    bool DevolucaoTotal,
    string? Observacao);

public sealed record AlterarStatusOcorrenciaRequest(
    StatusOcorrencia Status,
    string? Observacao);

public sealed record ConfirmarCreditoRequest(
    decimal? NumeroCredito,
    decimal? NumeroNotaCredito,
    string? Observacao);

public sealed record CancelarOcorrenciaRequest(string Motivo);

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,GERENTE")]
public class OcorrenciasController : ControllerBase
{
    private readonly IOcorrenciaRepository _repo;

    public OcorrenciasController(IOcorrenciaRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var codigoUsuario = GetCodigoUsuario();
        var ocorrencias = (await _repo.GetAllAsync(ct))
            .Where(x => x.Status != StatusOcorrencia.RASCUNHO || x.CodigoUsuario == codigoUsuario);

        return Ok(ocorrencias);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        [FromQuery] bool incluirItens = true,
        CancellationToken ct = default)
    {
        var ocorrencia = await _repo.GetByIdAsync(id, incluirItens, ct);
        if (ocorrencia is null)
            return NotFound("Ocorrência não encontrada.");

        if (!PodeVisualizar(ocorrencia))
            return Forbid();

        return Ok(ocorrencia);
    }

    [HttpGet("draft")]
    public async Task<IActionResult> GetDraft(
        [FromQuery] string codigoFilial,
        [FromQuery] decimal numeroNota,
        [FromQuery] string serieNota,
        CancellationToken ct)
    {
        var ocorrencia = await _repo.GetDraftAsync(
            codigoFilial, numeroNota, serieNota, GetCodigoUsuario(), true, ct);

        return ocorrencia is null ? NotFound() : Ok(ocorrencia);
    }

    [HttpPost("draft")]
    public async Task<IActionResult> CreateDraft(
        [FromBody] CriarOcorrenciaRequest request,
        CancellationToken ct)
    {
        if (request.NumeroNota <= 0 || request.NumeroTransacaoVenda <= 0)
            return BadRequest("Nota ou transação de venda inválida.");
        if (string.IsNullOrWhiteSpace(request.CodigoFilial) || string.IsNullOrWhiteSpace(request.SerieNota))
            return BadRequest("Filial e série são obrigatórias.");
        if (request.CodigoMotorista <= 0)
            return BadRequest("Informe o motorista responsável.");

        var ocorrencia = new Ocorrencia
        {
            CodigoFilial = request.CodigoFilial.Trim(),
            CodigoFilialNf = Limpar(request.CodigoFilialNf),
            NumeroNota = request.NumeroNota,
            SerieNota = request.SerieNota.Trim(),
            NumeroTransacaoVenda = request.NumeroTransacaoVenda,
            CodigoCliente = request.CodigoCliente,
            CodigoVendedor = request.CodigoVendedor,
            CodigoMotoristaNota = request.CodigoMotoristaNota,
            CodigoMotorista = request.CodigoMotorista,
            CodigoUsuario = GetCodigoUsuario(),
            DevolucaoTotal = request.DevolucaoTotal,
            Observacao = Limpar(request.Observacao)
        };

        try
        {
            var created = await _repo.CreateOrGetDraftAsync(ocorrencia, ct);
            return Ok(created);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateWithItems(
        [FromBody] CriarOcorrenciaComItensRequest request,
        CancellationToken ct)
    {
        if (request.NumeroNota <= 0 || request.NumeroTransacaoVenda <= 0)
            return BadRequest("Nota ou transação de venda inválida.");
        if (string.IsNullOrWhiteSpace(request.CodigoFilial) || string.IsNullOrWhiteSpace(request.SerieNota))
            return BadRequest("Filial e série são obrigatórias.");
        if (request.CodigoMotorista <= 0)
            return BadRequest("Informe o motorista responsável.");
        if (request.Itens is null || request.Itens.Count == 0)
            return BadRequest("Selecione pelo menos um item para a ocorrência.");

        var ocorrencia = new Ocorrencia
        {
            CodigoFilial = request.CodigoFilial.Trim(),
            CodigoFilialNf = Limpar(request.CodigoFilialNf),
            NumeroNota = request.NumeroNota,
            SerieNota = request.SerieNota.Trim(),
            NumeroTransacaoVenda = request.NumeroTransacaoVenda,
            CodigoCliente = request.CodigoCliente,
            CodigoVendedor = request.CodigoVendedor,
            CodigoMotoristaNota = request.CodigoMotoristaNota,
            CodigoMotorista = request.CodigoMotorista,
            CodigoUsuario = GetCodigoUsuario(),
            DevolucaoTotal = request.DevolucaoTotal,
            Observacao = Limpar(request.Observacao),
            Itens = request.Itens.Select(x => new OcorrenciaItem
            {
                NumeroTransacaoVenda = x.NumeroTransacaoVenda,
                NumeroItemNota = x.NumeroItemNota,
                CodigoProduto = x.CodigoProduto,
                QuantidadeOcorrencia = x.QuantidadeOcorrencia,
                CodigoMotivo = x.CodigoMotivo,
                Observacao = Limpar(x.Observacao)
            }).ToList()
        };

        try
        {
            var created = await _repo.CreateWithItemsAsync(ocorrencia, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] AtualizarOcorrenciaRequest request,
        CancellationToken ct)
    {
        var ocorrencia = await _repo.GetByIdAsync(id, false, ct);
        if (ocorrencia is null)
            return NotFound("Ocorrência não encontrada.");
        if (ocorrencia.CodigoUsuario != GetCodigoUsuario())
            return Forbid();
        if (request.CodigoMotorista <= 0)
            return BadRequest("Informe o motorista responsável.");

        ocorrencia.CodigoMotorista = request.CodigoMotorista;
        ocorrencia.DevolucaoTotal = request.DevolucaoTotal;
        ocorrencia.Observacao = Limpar(request.Observacao);

        try
        {
            await _repo.UpdateAsync(ocorrencia, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:int}/abrir")]
    public async Task<IActionResult> Abrir(int id, CancellationToken ct)
    {
        var ocorrencia = await _repo.GetByIdAsync(id, false, ct);
        if (ocorrencia is null)
            return NotFound("Ocorrência não encontrada.");
        if (ocorrencia.CodigoUsuario != GetCodigoUsuario())
            return Forbid();

        try
        {
            await _repo.FinalizarRascunhoAsync(id, GetCodigoUsuario(), ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:int}/status")]
    [Authorize(Roles = "ADMIN,GERENTE")]
    public async Task<IActionResult> AlterarStatus(
        int id,
        [FromBody] AlterarStatusOcorrenciaRequest request,
        CancellationToken ct)
    {
        if (request.Status is StatusOcorrencia.RASCUNHO or StatusOcorrencia.ABERTA or
            StatusOcorrencia.CREDITO_CONFERIDO or StatusOcorrencia.CANCELADA)
        {
            return BadRequest("Use o endpoint específico para esse status.");
        }

        try
        {
            await _repo.UpdateStatusAsync(
                id, request.Status, GetCodigoUsuario(), Limpar(request.Observacao), ct);
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

    [HttpPost("{id:int}/confirmar-credito")]
    [Authorize(Roles = "ADMIN,GERENTE")]
    public async Task<IActionResult> ConfirmarCredito(
        int id,
        [FromBody] ConfirmarCreditoRequest request,
        CancellationToken ct)
    {
        try
        {
            await _repo.ConfirmarCreditoAsync(
                id, request.NumeroCredito, request.NumeroNotaCredito,
                GetCodigoUsuario(), Limpar(request.Observacao), ct);
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

    [HttpPost("{id:int}/cancelar")]
    [Authorize(Roles = "ADMIN,GERENTE")]
    public async Task<IActionResult> Cancelar(
        int id,
        [FromBody] CancelarOcorrenciaRequest request,
        CancellationToken ct)
    {
        try
        {
            await _repo.CancelarAsync(id, GetCodigoUsuario(), request.Motivo, ct);
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ocorrencia = await _repo.GetByIdAsync(id, false, ct);
        if (ocorrencia is null)
            return NotFound("Ocorrência não encontrada.");
        if (ocorrencia.CodigoUsuario != GetCodigoUsuario())
            return Forbid();

        try
        {
            await _repo.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private bool PodeVisualizar(Ocorrencia ocorrencia) =>
        ocorrencia.Status != StatusOcorrencia.RASCUNHO ||
        ocorrencia.CodigoUsuario == GetCodigoUsuario();

    private int GetCodigoUsuario() => int.TryParse(
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"),
        out var id) ? id : 0;

    private static string? Limpar(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
