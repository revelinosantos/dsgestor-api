using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _repository;

    public ProdutosController(IProdutoRepository repository)
    {
        _repository = repository;
    }

    // GET api/Produto
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> GetAll()
        => Ok(await _repository.GetAllAsync());

    // GET api/Produto/10
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Produto>> GetById(int id)
    {
        var produto = await _repository.GetByIdAsync(id);
        if (produto == null)
            return NotFound();

        return Ok(produto);
    }

    // GET api/Produto/filial/01
    [HttpGet("filial/{codigoFilial}")]
    public async Task<ActionResult<IEnumerable<Produto>>> GetByFilial(string codigoFilial)
    {
        var itens = await _repository.GetByFilialAsync(codigoFilial);
        return Ok(itens); // ✅ 200 com [] é melhor pra UX do front
    }

    // GET api/Produto/filial/01/regiao/711
    [HttpGet("filial/{codigoFilial}/regiao/{codigoRegiao:int}")]
    public async Task<ActionResult<IEnumerable<Produto>>> GetByFilialRegiao(
        string codigoFilial,
        int codigoRegiao)
    {
        var itens = await _repository.GetByFilialAndRegiaoAsync(codigoFilial, codigoRegiao);
        return Ok(itens); // ✅
    }

    // GET api/Produto/filial/01/regiao/711/descricao?texto=ARROZ
    [HttpGet("filial/{codigoFilial}/regiao/{codigoRegiao:int}/descricao")]
    public async Task<ActionResult<IEnumerable<Produto>>> GetByFilialRegiaoDescricao(
        string codigoFilial,
        int codigoRegiao,
        [FromQuery] string texto)
    {
        var itens = await _repository.GetByFilialAndRegiaoAndDescricaoAsync(
            codigoFilial, codigoRegiao, texto);

        return Ok(itens); // ✅
    }

    // GET api/Produto/filial/01/regiao/711/id/10  (retorna 1 item)
    [HttpGet("filial/{codigoFilial}/regiao/{codigoRegiao:int}/id/{id:int}")]
    public async Task<ActionResult<Produto>> GetByFilialRegiaoById(
        string codigoFilial,
        int codigoRegiao,
        int id)
    {
        var produto = await _repository.GetByFilialAndRegiaoByIdAsync(
            codigoFilial, codigoRegiao, id);

        if (produto == null)
            return NotFound();

        return Ok(produto);
    }
}
