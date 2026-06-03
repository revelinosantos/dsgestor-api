using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartamentosController : ControllerBase
{
    private readonly IDepartamentoRepository _repository;

    public DepartamentosController(IDepartamentoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departamentos = await _repository.GetAllAsync();
        return Ok(departamentos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var departamento = await _repository.GetByIdAsync(id);
        if (departamento == null)
            return NotFound();

        return Ok(departamento);
    }
}
