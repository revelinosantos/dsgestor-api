using System.Security.Claims;
using DsGestor.Domain.Entities;
using DsGestor.Domain.Enums;
using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

public sealed record CriarOcorrenciaRomaneioRequest(
    string CodigoFilial,
    int CodigoMotorista,
    string? Observacao,
    IReadOnlyCollection<int> CodigosOcorrencias);

public sealed record CancelarOcorrenciaRomaneioRequest(string Motivo);

[ApiController]
[Route("api/ocorrencias-romaneios")]
[Authorize(Roles = "ADMIN,GERENTE")]
public sealed class OcorrenciasRomaneiosController : ControllerBase
{
    private readonly IOcorrenciaRomaneioRepository _repo;

    public OcorrenciasRomaneiosController(
        IOcorrenciaRomaneioRepository repo) =>
        _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? codigoFilial,
        [FromQuery] int? codigoMotorista,
        [FromQuery] StatusRomaneioOcorrencia? status,
        CancellationToken ct)
    {
        var romaneios = await _repo.GetAllAsync(
            codigoFilial,
            codigoMotorista,
            status,
            ct);

        return Ok(romaneios);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken ct)
    {
        var romaneio = await _repo.GetByIdAsync(id, ct);

        return romaneio is null
            ? NotFound("Romaneio não encontrado.")
            : Ok(romaneio);
    }

    [HttpGet("pendentes")]
    public async Task<IActionResult> GetPendentes(
        [FromQuery] int codigoMotorista,
        [FromQuery] string? codigoFilial,
        CancellationToken ct)
    {
        if (codigoMotorista <= 0)
            return BadRequest("Informe o motorista.");

        var ocorrencias = await _repo.GetOcorrenciasPendentesAsync(
            codigoMotorista,
            codigoFilial,
            ct);

        return Ok(ocorrencias);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CriarOcorrenciaRomaneioRequest request,
        CancellationToken ct)
    {
        if (request.CodigosOcorrencias is null ||
            request.CodigosOcorrencias.Count == 0)
        {
            return BadRequest(
                "Selecione pelo menos uma ocorrência para o romaneio.");
        }

        var romaneio = new OcorrenciaRomaneio
        {
            CodigoFilial = request.CodigoFilial,
            CodigoMotorista = request.CodigoMotorista,
            CodigoUsuario = GetCodigoUsuario(),
            Observacao = Limpar(request.Observacao)
        };

        try
        {
            var created = await _repo.CreateAsync(
                romaneio,
                request.CodigosOcorrencias,
                ct);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
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
    public async Task<IActionResult> Cancelar(
        int id,
        [FromBody] CancelarOcorrenciaRomaneioRequest request,
        CancellationToken ct)
    {
        try
        {
            await _repo.CancelarAsync(
                id,
                GetCodigoUsuario(),
                request.Motivo,
                ct);

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

    private int GetCodigoUsuario() => int.TryParse(
        User.FindFirstValue(ClaimTypes.NameIdentifier) ??
        User.FindFirstValue("sub"),
        out var id) ? id : 0;

    private static string? Limpar(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
