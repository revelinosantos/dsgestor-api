using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VendedoresController : ControllerBase
{
    private readonly IVendedorRepository _repository;

    public VendedoresController(IVendedorRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vendedores = await _repository.GetAllAsync();
        return Ok(vendedores);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vendedor = await _repository.GetByIdAsync(id);
        if (vendedor == null)
            return NotFound();

        return Ok(vendedor);
    }
}
