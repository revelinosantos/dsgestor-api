using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsGestor.Api.Controllers;

[ApiController]
[Route("api/notas-fiscais-ocorrencia")]
[Authorize(Roles = "ADMIN,GERENTE")]
public class NotasFiscaisOcorrenciaController : ControllerBase
{
    private readonly INotaFiscalOcorrenciaRepository _repo;

    public NotasFiscaisOcorrenciaController(INotaFiscalOcorrenciaRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] decimal numeroNota,
        [FromQuery] string codigoFilial,
        [FromQuery] string serie,
        CancellationToken ct)
    {
        var linhas = await _repo.PesquisarAsync(numeroNota, codigoFilial, serie, ct);
        if (linhas.Count == 0)
            return NotFound("Nota fiscal não encontrada.");

        var itens = new List<object>();
        foreach (var linha in linhas)
        {
            var possuiOcorrencia = await _repo.ItemPossuiOcorrenciaAsync(
                linha.NumeroTransacaoVenda, linha.NumeroTransacaoItem, ct);

            itens.Add(new
            {
                linha.NumeroTransacaoItem,
                linha.CodigoProduto,
                linha.DescricaoProduto,
                linha.Unidade,
                linha.Quantidade,
                linha.PrecoUnitario,
                linha.ValorTotalItem,
                PossuiOcorrencia = possuiOcorrencia
            });
        }

        var nota = linhas[0];
        return Ok(new
        {
            nota.DataSaida,
            nota.CodigoFilial,
            nota.CodigoFilialNf,
            nota.NumeroNota,
            nota.Serie,
            nota.NumeroTransacaoVenda,
            nota.CodigoCliente,
            nota.Cliente,
            nota.Fantasia,
            nota.Cidade,
            nota.CodigoVendedor,
            nota.Vendedor,
            nota.CodigoMotorista,
            nota.Motorista,
            Itens = itens
        });
    }
}
