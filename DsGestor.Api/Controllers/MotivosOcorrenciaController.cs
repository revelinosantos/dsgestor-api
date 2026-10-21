using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

[ApiController]
[Route("api/motivos-ocorrencia")]
[Authorize(Roles = "ADMIN,GERENTE")]
public class MotivosOcorrenciaController : ControllerBase
{
    private readonly IMotivoOcorrenciaRepository _repo;

    public MotivosOcorrenciaController(IMotivoOcorrenciaRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(await _repo.GetAtivosAsync(ct));
}
