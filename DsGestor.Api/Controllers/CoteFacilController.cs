using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DsGestor.Application.Dtos.CoteFacil;
using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace DsGestor.Api.Controllers;

[ApiController]
[Route("v1")]
public class CoteFacilController : ControllerBase
{
    private readonly ICoteFacilPedidoRepository _pedidoRepository;
    private readonly ICoteFacilConsultaRepository _consultaRepository;
    private readonly ICoteFacilProdutoRepository _produtoRepository;
    private readonly IConfiguration _configuration;

    public CoteFacilController(
        ICoteFacilPedidoRepository pedidoRepository,
        ICoteFacilConsultaRepository consultaRepository,
        ICoteFacilProdutoRepository produtoRepository,
        IConfiguration configuration)
    {
        _pedidoRepository = pedidoRepository;
        _consultaRepository = consultaRepository;
        _produtoRepository = produtoRepository;
        _configuration = configuration;
    }
    
    [AllowAnonymous]
    [HttpPost("")]
    public IActionResult Autenticar([FromBody] CoteFacilAuthRequest request)
    {
        var erro = ValidarAuthRequest(request);

        if (!string.IsNullOrWhiteSpace(erro))
        {
            return BadRequest(new CoteFacilAuthResponse
            {
                Sucesso = false,
                Mensagem = erro
            });
        }

        var usernameConfig = _configuration["CoteFacil:Username"];
        var passwordConfig = _configuration["CoteFacil:Password"];
        var clientSecretConfig = _configuration["CoteFacil:ClientSecret"];

        var credenciaisInvalidas =
            !string.Equals(request.Username, usernameConfig, StringComparison.Ordinal) ||
            !string.Equals(request.Password, passwordConfig, StringComparison.Ordinal) ||
            !string.Equals(request.ClientSecret, clientSecretConfig, StringComparison.Ordinal);

        if (credenciaisInvalidas)
        {
            return Unauthorized(new CoteFacilAuthResponse
            {
                Sucesso = false,
                Mensagem = "Credenciais inválidas."
            });
        }

        var tokenExpirationMinutes = _configuration.GetValue<int?>("CoteFacil:TokenExpirationMinutes") ?? 120;

        var expiresAt = DateTime.UtcNow.AddMinutes(tokenExpirationMinutes);

        var token = GerarTokenCoteFacil(expiresAt);

        return Ok(new CoteFacilAuthResponse
        {
            Sucesso = true,
            Token = token,
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = tokenExpirationMinutes * 60,
            Mensagem = "Autenticação realizada com sucesso."
        });
    }

    [Authorize(Policy = "CoteFacil")]
    [HttpPost("inserirpedido")]
    public async Task<IActionResult> InserirPedido(
        [FromQuery] int page,
        [FromQuery] int size,
        [FromBody] CoteFacilInserirPedidoRequest request)
    {
        NormalizarRequest(request);

        var erro = ValidarRequest(request);
        if (!string.IsNullOrWhiteSpace(erro))
        {
            return BadRequest(new CoteFacilInserirPedidoResponse
            {
                Sucesso = false,
                Status = "ERRO_VALIDACAO",
                Mensagem = erro,
                PedidoCoteFacil = request.Cotefacil?.PedidoCoteFacil,
                CotacaoCoteFacil = request.Cotefacil?.CotacaoCoteFacil
            });
        }

        var existente = await _pedidoRepository.GetPedidoExistenteAsync(
            request.Distribuidor!.CnpjDistribuidor!,
            request.Cliente!.CnpjCliente!,
            request.Cotefacil?.CotacaoCoteFacil,
            request.Cotefacil?.PedidoCoteFacil);

        if (existente is not null)
        {
            return Ok(new CoteFacilInserirPedidoResponse
            {
                Sucesso = true,
                Status = "DUPLICADO",
                Mensagem = "Pedido já recebido anteriormente. Nenhum novo pedido foi criado.",
                IdPedido = existente.IdPedido,
                PedidoCoteFacil = existente.PedidoCoteFacil,
                CotacaoCoteFacil = existente.CotacaoCoteFacil,
                NumPedWinThor = existente.NumpedWinthor
            });
        }

        var jsonRequest = JsonSerializer.Serialize(request);
        var hash = GerarSha256(jsonRequest);

        var pedido = MontarPedido(request, jsonRequest, hash);

        var criado = await _pedidoRepository.AddAsync(pedido);

        var response = new CoteFacilInserirPedidoResponse
        {
            Sucesso = true,
            Status = "RECEBIDO",
            Mensagem = "Pedido recebido e salvo em staging. Ainda não importado para o WinThor.",
            IdPedido = criado.IdPedido,
            PedidoCoteFacil = criado.PedidoCoteFacil,
            CotacaoCoteFacil = criado.CotacaoCoteFacil,
            NumPedWinThor = criado.NumpedWinthor
        };

        var jsonResponse = JsonSerializer.Serialize(response);

        await _pedidoRepository.AtualizarRetornoAsync(
            criado.IdPedido,
            "RECEBIDO",
            jsonResponse,
            null);

        return Ok(response);
    }

    [Authorize(Policy = "CoteFacil")]
    [HttpPost("confirmacaodospedidos")]
    public async Task<IActionResult> ConfirmacaoDosPedidos(
        [FromQuery] int page,
        [FromQuery] int size,
        [FromBody] CoteFacilConfirmacaoPedidoRequest request)
    {
        var cnpjDistribuidor = SomenteNumeros(request.CnpjDistribuidor);

        if (string.IsNullOrWhiteSpace(cnpjDistribuidor))
            return BadRequest("Campo 'cnpjDistribuidor' não informado.");

        var dataInicial = ConverterData(request.DataInicial);
        var dataFinal = ConverterData(request.DataFinal);

        var pedidos = await _pedidoRepository.GetConfirmacaoAsync(
            cnpjDistribuidor,
            dataInicial,
            dataFinal,
            request.Pedidos);

        var response = new CoteFacilConfirmacaoPedidoResponse
        {
            Sucesso = true,
            Pedidos = pedidos.Select(x => new CoteFacilPedidoConfirmadoDto
            {
                IdPedido = x.IdPedido,
                PedidoCoteFacil = x.PedidoCoteFacil,
                CotacaoCoteFacil = x.CotacaoCoteFacil,
                PedidoCliente = x.PedidoCliente,
                CnpjCliente = x.CnpjCliente,
                Status = x.Status,
                NumPedWinThor = x.NumpedWinthor,
                MensagemErro = x.MensagemErro,
                DataCriacao = x.DataCriacao,
                DataProcessamento = x.DataProcessamento
            }).ToList()
        };

        return Ok(response);
    }

    [Authorize(Policy = "CoteFacil")]
    [HttpPost("processarpedido/{idPedido:long}")]
    public async Task<IActionResult> ProcessarPedidoWinThor(
        long idPedido,
        CancellationToken ct)
    {
        try
        {
            var numped = await _pedidoRepository.ImportarPedidoWinThorAsync(idPedido, ct);

            return Ok(new
            {
                sucesso = true,
                idPedido,
                numpedWinThor = numped,
                mensagem = "Pedido importado para o WinThor com sucesso."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                sucesso = false,
                idPedido,
                mensagem = ex.Message
            });
        }
    }
    private static CoteFacilPedido MontarPedido(
        CoteFacilInserirPedidoRequest request,
        string jsonRequest,
        string hash)
    {
        var pedido = new CoteFacilPedido
        {
            CnpjDistribuidor = request.Distribuidor!.CnpjDistribuidor!,
            CodigoDistribuidor = request.Distribuidor.CodigoDistribuidor,

            CnpjCliente = request.Cliente!.CnpjCliente!,
            CodcliWinthor = request.Cliente.CodigoCliente,

            CotacaoCoteFacil = request.Cotefacil?.CotacaoCoteFacil,
            PedidoCoteFacil = request.Cotefacil?.PedidoCoteFacil,
            PedidoCliente = request.Cotefacil?.PedidoCliente,

            CodigoPromocao = request.Promocao?.CodigoPromocao,
            CodigoPrazoPagamento = request.Pagamento?.CodigoPrazoPagamento,
            CodigoCondicaoComercial = request.Pagamento?.CodigoCondicaoComercial,

            Status = "RECEBIDO",
            TentativasProcessamento = 0,
            HashRequisicao = hash,
            JsonRequest = jsonRequest,
            DataCriacao = DateTime.Now
        };

        var sequencia = 0;

        foreach (var item in request.Produtos)
        {
            sequencia++;

            var quantidade = item.QuantidadeSolicitada ?? 0;
            var valorUnitario = item.ValorUnitario ?? 0;

            pedido.Itens.Add(new CoteFacilPedidoItem
            {
                Sequencia = sequencia,

                Ean = item.Ean,
                Dun = item.Dun,
                CodigoProdutoCoteFacil = item.CodigoProduto,

                QuantidadeSolicitada = quantidade,

                ValorFabrica = item.ValorFabrica,
                ValorUnitario = item.ValorUnitario,
                ValorUnitarioNf = item.ValorUnitarioNf,
                ValorUnitarioBoleto = item.ValorUnitarioBoleto,
                ValorTotalItem = quantidade * valorUnitario,

                DescontoAdicional = item.DescontoAdicional,
                ValorDescAdicional = item.ValorDescontoAdicional,

                DescontoBonificacao = item.DescontoBonificacao,
                ValorDescBonificacao = item.ValorDescontoBonificacao,

                DescontoComercial = item.DescontoComercial,
                ValorDescComercial = item.ValorDescontoComercial,

                DescontoFinanceiro = item.DescontoFinanceiro,
                ValorDescFinanceiro = item.ValorDescontoFinanceiro,

                CodigoPromocao = item.CodigoPromocao,

                Status = "RECEBIDO",
                DataCriacao = DateTime.Now
            });
        }

        return pedido;
    }

    [Authorize(Policy = "CoteFacil")]
    [HttpGet("consultafilialdistribuidor")]
    public async Task<IActionResult> ConsultarFiliaisDistribuidor()
    {
        var filiais = await _consultaRepository.GetFiliaisAsync();

        var response = filiais.Select(x => new CoteFacilFilialDistribuidorDto
        {
            CodigoFilial = x.CodigoFilial,
            CnpjDistribuidor = SomenteNumeros(x.Cnpj) ?? string.Empty,
            RazaoSocial = x.RazaoSocial,
            NomeFantasia = x.NomeFantasia,
            Cidade = x.Cidade,
            Uf = x.Uf
        }).ToList();

        return Ok(new
        {
            sucesso = true,
            mensagem = "Filiais consultadas com sucesso.",
            content = response
        });
    }

    [Authorize(Policy = "CoteFacil")]
    [HttpPost("cliente")]
    public async Task<IActionResult> ConsultarCliente(
        [FromQuery] int page,
        [FromQuery] int size,
        [FromBody] CoteFacilClienteRequest request)
    {
        page = NormalizarPage(page);
        size = NormalizarSize(size);

        request.CnpjDistribuidor = SomenteNumeros(request.CnpjDistribuidor);
        request.CnpjCliente = SomenteNumeros(request.CnpjCliente);

        if (string.IsNullOrWhiteSpace(request.CnpjDistribuidor))
            return BadRequest("Campo 'cnpjDistribuidor' não informado.");

        var (clientes, total) = await _consultaRepository.GetClientesAsync(
            request.CnpjDistribuidor,
            request.CnpjCliente,
            page,
            size);

        var content = clientes.Select(x => new CoteFacilClienteDto
        {
            CodigoCliente = x.Id,
            CnpjCliente = SomenteNumeros(x.CnpjCpf) ?? string.Empty,
            RazaoSocial = x.RazaoSocial,
            NomeFantasia = x.NomeFantasia,
            Endereco = x.Endereco,
            Bairro = x.Bairro,
            Cidade = x.Cidade,
            Uf = x.Estado,
            Telefone = x.Fone,
            CodigoFilial = x.CodigoFilial,
            CodigoVendedor = x.CodigoVendedor,
            NomeVendedor = x.NomeVendedor,
            NumRegiao = x.NumRegiao,
            NomeRegiao = x.NomeRegiao,
            LimiteCredito = x.LimiteCredito
        }).ToList();

        return Ok(new CoteFacilPagedResponse<CoteFacilClienteDto>
        {
            Sucesso = true,
            Mensagem = "Clientes consultados com sucesso.",
            Page = page,
            Size = size,
            TotalElements = total,
            Content = content
        });
    }

    [Authorize(Policy = "CoteFacil")]
    [HttpPost("condicaopagamento")]
    public async Task<IActionResult> ConsultarCondicaoPagamento(
        [FromQuery] int page,
        [FromQuery] int size,
        [FromBody] CoteFacilCondicaoPagamentoRequest request)
    {
        page = NormalizarPage(page);
        size = NormalizarSize(size);

        request.CnpjDistribuidor = SomenteNumeros(request.CnpjDistribuidor);

        if (string.IsNullOrWhiteSpace(request.CnpjDistribuidor))
            return BadRequest("Campo 'cnpjDistribuidor' não informado.");

        var codigoCondicao =
            request.CodigoCondicaoPagamento ??
            request.CondicaoPagamento?.CodigoCondicaoPagamento ??
            request.CondicaoPagamento?.CodigoPrazoPagamento ??
            request.CondicaoPagamento?.CodigoCondicaoComercial;

        var (condicoes, total) = await _consultaRepository.GetCondicoesPagamentoAsync(
            request.CnpjDistribuidor,
            codigoCondicao,
            page,
            size);

        var content = condicoes.Select(x => new CoteFacilCondicaoPagamentoDto
        {
            CodigoCondicaoPagamento = x.CodigoCondicaoPagamento,
            CodigoPrazoPagamento = x.CodigoCondicaoPagamento,
            CodigoCondicaoComercial = x.CodigoCondicaoPagamento,
            Descricao = x.Descricao,
            NumDias = x.NumDias
        }).ToList();

        return Ok(new CoteFacilPagedResponse<CoteFacilCondicaoPagamentoDto>
        {
            Sucesso = true,
            Mensagem = "Condições de pagamento consultadas com sucesso.",
            Page = page,
            Size = size,
            TotalElements = total,
            Content = content
        });
    }

    [Authorize(Policy = "CoteFacil")]
    [HttpPost("produto")]
    public async Task<IActionResult> ConsultarProduto(
        [FromQuery] int page,
        [FromQuery] int size,
        [FromBody] CoteFacilProdutoRequest request)
    {
        page = NormalizarPage(page);
        size = NormalizarSize(size);

        var cnpjDistribuidor = SomenteNumeros(request.CnpjDistribuidor);

        if (string.IsNullOrWhiteSpace(cnpjDistribuidor))
            return BadRequest("Campo 'cnpjDistribuidor' não informado.");

        var codigos = ExtrairCodigosProduto(request.Produto);
        var eans = ExtrairEans(request.Produto);
        var duns = ExtrairDuns(request.Produto);

        var (dados, total) = await _produtoRepository.ConsultarAsync(
            cnpjDistribuidor,
            codigos,
            eans,
            duns,
            request.Produto?.Descricao,
            page,
            size);

        var response = new CoteFacilResponseConteudo<CoteFacilProdutoCatalogoDto>
        {
            CnpjDistribuidor = cnpjDistribuidor,
            Page = page,
            Size = size,
            TotalElements = total,
            Conteudo = dados.Select(x => new CoteFacilProdutoCatalogoDto
            {
                CnpjFabricante = SomenteNumeros(x.CnpjFabricante),
                Fabricante = x.Fabricante,
                CnpjFornecedor = SomenteNumeros(x.CnpjFornecedor),
                NomeFornecedor = x.NomeFornecedor,
                Ativo = string.IsNullOrWhiteSpace(x.Ativo) ? "S" : x.Ativo,
                CodigoProduto = x.CodigoProduto.ToString(),
                TipoProduto = x.TipoProduto,
                Ncm = x.Ncm,
                Ean = x.Ean,
                Dun = x.Dun,
                EmbalagemEan = x.EmbalagemEan,
                QtdeEan = x.QtdeEan,
                EmbalagemDun = x.EmbalagemDun,
                QtdeDun = x.QtdeDun,
                Descricao = x.Descricao,
                Categoria1 = x.Categoria1,
                Categoria2 = x.Categoria2,
                Categoria3 = x.Categoria3,
                Imagem = x.Imagem,
                Observacao = x.Observacao
            }).ToList()
        };

        return Ok(response);
    }

    [Authorize(Policy = "CoteFacil")]
    [HttpPost("consultapreco")]
    public async Task<IActionResult> ConsultarPreco(
        [FromQuery] int page,
        [FromQuery] int size,
        [FromBody] CoteFacilConsultaPrecoRequest request)
    {
        page = NormalizarPage(page);
        size = NormalizarSize(size);

        var cnpjDistribuidor = SomenteNumeros(request.Cliente?.CnpjDistribuidor);

        if (string.IsNullOrWhiteSpace(cnpjDistribuidor))
            return BadRequest("Campo 'cliente.cnpjDistribuidor' não informado.");

        var codigos = ExtrairCodigosProduto(request.Produto);
        var eans = ExtrairEans(request.Produto);
        var duns = ExtrairDuns(request.Produto);

        var (dados, total) = await _produtoRepository.ConsultarAsync(
            cnpjDistribuidor,
            codigos,
            eans,
            duns,
            null,
            page,
            size);

        var response = new CoteFacilResponseConteudo<CoteFacilPrecoDto>
        {
            CnpjDistribuidor = cnpjDistribuidor,
            Page = page,
            Size = size,
            TotalElements = total,
            Conteudo = dados.Select(x =>
            {
                var valorFabrica = x.PrecoFabrica ?? 0;
                var valorVenda = x.PrecoVenda ?? valorFabrica;
                var desconto = x.DescontoMaximo ?? 0;
                var valorDesconto = Math.Round(valorFabrica - valorVenda, 2);

                return new CoteFacilPrecoDto
                {
                    CodigoProduto = x.CodigoProduto.ToString(),
                    Ean = x.Ean,
                    Dun = x.Dun,
                    Descricao = x.Descricao,
                    ValorFabrica = valorFabrica,
                    DescontoComercial = desconto,
                    ValorDescontoComercial = valorDesconto,
                    ValorUnitario = valorVenda,
                    ValorUnitarioNf = valorVenda,
                    ValorUnitarioBoleto = valorVenda
                };
            }).ToList()
        };

        return Ok(response);
    }

    [Authorize(Policy = "CoteFacil")]
    [HttpPost("estoqueproduto")]
    public async Task<IActionResult> ConsultarEstoqueProduto(
        [FromQuery] int page,
        [FromQuery] int size,
        [FromBody] CoteFacilEstoqueProdutoRequest request)
    {
        page = NormalizarPage(page);
        size = NormalizarSize(size);

        var cnpjDistribuidor = SomenteNumeros(request.CnpjDistribuidor);

        if (string.IsNullOrWhiteSpace(cnpjDistribuidor))
            return BadRequest("Campo 'cnpjDistribuidor' não informado.");

        var codigos = ExtrairCodigosProduto(request.Produto);
        var eans = ExtrairEans(request.Produto);
        var duns = ExtrairDuns(request.Produto);

        var (dados, total) = await _produtoRepository.ConsultarAsync(
            cnpjDistribuidor,
            codigos,
            eans,
            duns,
            request.Produto?.Descricao,
            page,
            size);

        var response = new CoteFacilResponseConteudo<CoteFacilEstoqueProdutoDto>
        {
            CnpjDistribuidor = cnpjDistribuidor,
            Page = page,
            Size = size,
            TotalElements = total,
            Conteudo = dados.Select(x => new CoteFacilEstoqueProdutoDto
            {
                CodigoProduto = x.CodigoProduto.ToString(),
                Ean = x.Ean,
                Dun = x.Dun,
                Descricao = x.Descricao,
                Estoque = x.EstoqueGeral ?? 0,
                EstoqueDisponivel = x.EstoqueDisponivel ?? 0,
                CodigoFilial = x.Codfilial
            }).ToList()
        };

        return Ok(response);
    }
    private static string? ValidarRequest(CoteFacilInserirPedidoRequest request)
    {
        if (request.Distribuidor is null)
            return "Bloco 'distribuidor' não informado.";

        if (string.IsNullOrWhiteSpace(request.Distribuidor.CnpjDistribuidor))
            return "Campo 'cnpjDistribuidor' não informado.";

        if (request.Cliente is null)
            return "Bloco 'cliente' não informado.";

        if (string.IsNullOrWhiteSpace(request.Cliente.CnpjCliente))
            return "Campo 'cnpjCliente' não informado.";

        if (request.Cotefacil is null)
            return "Bloco 'cotefacil' não informado.";

        if (request.Cotefacil.PedidoCoteFacil is null)
            return "Campo 'pedidoCoteFacil' não informado.";

        if (request.Produtos.Count == 0)
            return "Nenhum produto informado no pedido.";

        return null;
    }

    private static void NormalizarRequest(CoteFacilInserirPedidoRequest request)
    {
        if (request.Distribuidor is not null)
            request.Distribuidor.CnpjDistribuidor = SomenteNumeros(request.Distribuidor.CnpjDistribuidor);

        if (request.Cliente is not null)
            request.Cliente.CnpjCliente = SomenteNumeros(request.Cliente.CnpjCliente);

        foreach (var item in request.Produtos)
        {
            item.Ean = SomenteNumeros(item.Ean);
            item.Dun = SomenteNumeros(item.Dun);
        }
    }

    private static string? SomenteNumeros(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return valor;

        return new string(valor.Where(char.IsDigit).ToArray());
    }

    private static string GerarSha256(string texto)
    {
        var bytes = Encoding.UTF8.GetBytes(texto);
        var hashBytes = SHA256.HashData(bytes);

        return Convert.ToHexString(hashBytes);
    }

    private static DateTime? ConverterData(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return null;

        if (DateTime.TryParse(valor, out var data))
            return data.Date;

        return null;
    }

    private static string? ValidarAuthRequest(CoteFacilAuthRequest request)
    {
        if (request is null)
            return "Body da requisição não informado.";

        if (string.IsNullOrWhiteSpace(request.Username))
            return "Campo 'username' não informado.";

        if (string.IsNullOrWhiteSpace(request.Password))
            return "Campo 'password' não informado.";

        if (string.IsNullOrWhiteSpace(request.ClientSecret))
            return "Campo 'clientSecret' não informado.";

        return null;
    }

    private string GerarTokenCoteFacil(DateTime expiresAt)
    {
        var jwtKey = ObterJwtKey();
        var issuer = ObterJwtIssuer();
        var audience = ObterJwtAudience();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "cotefacil"),
            new Claim(ClaimTypes.NameIdentifier, "cotefacil"),
            new Claim(ClaimTypes.Name, "Cote Fácil"),
            new Claim("integration", "cotefacil")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string ObterJwtKey()
    {
        var key =
            _configuration["Jwt:Key"] ??
            _configuration["Jwt:Secret"] ??
            _configuration["JwtSettings:Key"] ??
            _configuration["JwtSettings:Secret"];

        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Chave JWT não configurada. Verifique Jwt:Key ou JwtSettings:Secret.");

        return key;
    }

    private string? ObterJwtIssuer()
    {
        return
            _configuration["Jwt:Issuer"] ??
            _configuration["JwtSettings:Issuer"];
    }

    private string? ObterJwtAudience()
    {
        return
            _configuration["Jwt:Audience"] ??
            _configuration["JwtSettings:Audience"];
    }

    private static int NormalizarPage(int page)
    {
        return page < 0 ? 0 : page;
    }

    private static int NormalizarSize(int size)
    {
        if (size <= 0) return 10;
        if (size > 100) return 100;
        return size;
    }

    private static List<int> ExtrairCodigosProduto(CoteFacilProdutoFiltroDto? produto)
    {
        if (produto is null)
            return new List<int>();

        if (int.TryParse(produto.CodigoProduto, out var codigo))
            return new List<int> { codigo };

        return new List<int>();
    }

    private static List<int> ExtrairCodigosProduto(IEnumerable<CoteFacilProdutoFiltroDto> produtos)
    {
        return produtos
            .Where(x => !string.IsNullOrWhiteSpace(x.CodigoProduto))
            .Select(x => int.TryParse(x.CodigoProduto, out var codigo) ? codigo : (int?)null)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();
    }

    private static List<string> ExtrairEans(CoteFacilProdutoFiltroDto? produto)
    {
        if (produto?.Ean is null)
            return new List<string>();

        var ean = SomenteNumeros(produto.Ean);

        return string.IsNullOrWhiteSpace(ean)
            ? new List<string>()
            : new List<string> { ean };
    }

    private static List<string> ExtrairEans(IEnumerable<CoteFacilProdutoFiltroDto> produtos)
    {
        return produtos
            .Select(x => SomenteNumeros(x.Ean))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .Distinct()
            .ToList();
    }

    private static List<string> ExtrairDuns(CoteFacilProdutoFiltroDto? produto)
    {
        if (produto?.Dun is null)
            return new List<string>();

        var dun = SomenteNumeros(produto.Dun);

        return string.IsNullOrWhiteSpace(dun)
            ? new List<string>()
            : new List<string> { dun };
    }

    private static List<string> ExtrairDuns(IEnumerable<CoteFacilProdutoFiltroDto> produtos)
    {
        return produtos
            .Select(x => SomenteNumeros(x.Dun))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .Distinct()
            .ToList();
    }
}