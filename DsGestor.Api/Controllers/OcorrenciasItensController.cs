using System.Security.Claims;
using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

public sealed record CriarOcorrenciaItemRequest(
    decimal NumeroTransacaoVenda,
    int NumeroItemNota,
    int CodigoProduto,
    decimal QuantidadeOcorrencia,
    int CodigoMotivo,
    string? Observacao);

public sealed record AtualizarOcorrenciaItemRequest(
    decimal QuantidadeOcorrencia,
    int CodigoMotivo,
    string? Observacao);

[ApiController]
[Route("api/ocorrencias/{idOcorrencia:int}/itens")]
[Authorize(Roles = "ADMIN,GERENTE")]
public class OcorrenciasItensController : ControllerBase
{
    private readonly IOcorrenciaItemRepository _repo;
    private readonly IOcorrenciaRepository _ocorrenciaRepo;

    public OcorrenciasItensController(
        IOcorrenciaItemRepository repo,
        IOcorrenciaRepository ocorrenciaRepo)
    {
        _repo = repo;
        _ocorrenciaRepo = ocorrenciaRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int idOcorrencia, CancellationToken ct)
    {
        var validacao = await ValidarAcessoAsync(idOcorrencia, false, ct);
        if (validacao is not null)
            return validacao;

        return Ok(await _repo.GetByOcorrenciaAsync(idOcorrencia, ct));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        int idOcorrencia,
        [FromBody] CriarOcorrenciaItemRequest request,
        CancellationToken ct)
    {
        var validacao = await ValidarAcessoAsync(idOcorrencia, true, ct);
        if (validacao is not null)
            return validacao;

        var item = new OcorrenciaItem
        {
            CodigoOcorrencia = idOcorrencia,
            NumeroTransacaoVenda = request.NumeroTransacaoVenda,
            NumeroItemNota = request.NumeroItemNota,
            CodigoProduto = request.CodigoProduto,
            QuantidadeOcorrencia = request.QuantidadeOcorrencia,
            CodigoMotivo = request.CodigoMotivo,
            Observacao = Limpar(request.Observacao)
        };

        try
        {
            return Ok(await _repo.CreateAsync(item, ct));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int idOcorrencia,
        int id,
        [FromBody] AtualizarOcorrenciaItemRequest request,
        CancellationToken ct)
    {
        var validacao = await ValidarAcessoAsync(idOcorrencia, true, ct);
        if (validacao is not null)
            return validacao;

        var item = new OcorrenciaItem
        {
            Id = id,
            CodigoOcorrencia = idOcorrencia,
            QuantidadeOcorrencia = request.QuantidadeOcorrencia,
            CodigoMotivo = request.CodigoMotivo,
            Observacao = Limpar(request.Observacao)
        };

        try
        {
            await _repo.UpdateAsync(item, ct);
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
    public async Task<IActionResult> Delete(
        int idOcorrencia,
        int id,
        CancellationToken ct)
    {
        var validacao = await ValidarAcessoAsync(idOcorrencia, true, ct);
        if (validacao is not null)
            return validacao;

        try
        {
            await _repo.DeleteAsync(id, idOcorrencia, ct);
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

    private async Task<IActionResult?> ValidarAcessoAsync(
        int idOcorrencia,
        bool exigirRascunho,
        CancellationToken ct)
    {
        var ocorrencia = await _ocorrenciaRepo.GetByIdAsync(idOcorrencia, false, ct);
        if (ocorrencia is null)
            return NotFound("Ocorrência não encontrada.");

        if (ocorrencia.Status == StatusOcorrencia.RASCUNHO &&
            ocorrencia.CodigoUsuario != GetCodigoUsuario())
            return Forbid();

        if (exigirRascunho && ocorrencia.Status != StatusOcorrencia.RASCUNHO)
            return BadRequest("Itens só podem ser alterados em RASCUNHO.");

        return null;
    }

    private int GetCodigoUsuario() => int.TryParse(
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"),
        out var id) ? id : 0;

    private static string? Limpar(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
