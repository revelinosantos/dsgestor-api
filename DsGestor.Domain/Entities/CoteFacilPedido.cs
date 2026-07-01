namespace DsGestor.Domain.Entities;

public class CoteFacilPedido
{
    public long IdPedido { get; set; }

    public string CnpjDistribuidor { get; set; } = string.Empty;
    public int? CodigoDistribuidor { get; set; }
    public string? CodfilialWinthor { get; set; }

    public string CnpjCliente { get; set; } = string.Empty;
    public int? CodcliWinthor { get; set; }

    public long? CotacaoCoteFacil { get; set; }
    public long? PedidoCoteFacil { get; set; }
    public string? PedidoCliente { get; set; }

    public long? CodigoPromocao { get; set; }
    public int? CodigoPrazoPagamento { get; set; }
    public int? CodigoCondicaoComercial { get; set; }

    public int? CodplpagWinthor { get; set; }
    public string? CodcobWinthor { get; set; }
    public int? CodusurWinthor { get; set; }

    public long? NumpedWinthor { get; set; }
    public DateTime? DataImportacaoWinthor { get; set; }

    public string Status { get; set; } = "RECEBIDO";
    public int TentativasProcessamento { get; set; }

    public string? HashRequisicao { get; set; }
    public string? JsonRequest { get; set; }
    public string? JsonResponse { get; set; }
    public string? MensagemErro { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime? DataAtualizacao { get; set; }
    public DateTime? DataProcessamento { get; set; }
    public string? UsuarioCriacao { get; set; }

    public ICollection<CoteFacilPedidoItem> Itens { get; set; } = new List<CoteFacilPedidoItem>();
}