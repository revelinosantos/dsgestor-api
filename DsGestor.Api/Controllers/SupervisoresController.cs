using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupervisoresController : ControllerBase
{
    private readonly ISupervisorRepository _repository;

    public SupervisoresController(ISupervisorRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var supervisores = await _repository.GetAllAsync();
        return Ok(supervisores);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var supervisor = await _repository.GetByIdAsync(id);
        if (supervisor == null)
            return NotFound();

        return Ok(supervisor);
    }
}
