using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteRepository _repository;

    public ClientesController(IClienteRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetAll()
    {
        var clientes = await _repository.GetAllAsync();
        return Ok(clientes);
    }

    [HttpGet("{int}")]
    public async Task<ActionResult<Cliente>> GetById(int id)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null) return NotFound();
        return Ok(cliente);
    }

    [HttpGet("vendedor/{codigoVendedor:int}")]
    public async Task<IActionResult> GetByVendedor(int codigoVendedor)
    {
        var clientes = await _repository.GetByVendedorAsync(codigoVendedor);
        return Ok(clientes);
    }

}
