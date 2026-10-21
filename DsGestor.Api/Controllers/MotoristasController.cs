using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,GERENTE")]
public class MotoristasController : ControllerBase
{
    private readonly IMotoristaRepository _repo;

    public MotoristasController(IMotoristaRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _repo.GetAllAsync());

    [HttpGet("{matricula:int}")]
    public async Task<IActionResult> GetById(int matricula)
    {
        var motorista = await _repo.GetByIdAsync(matricula);
        return motorista is null ? NotFound() : Ok(motorista);
    }
}
