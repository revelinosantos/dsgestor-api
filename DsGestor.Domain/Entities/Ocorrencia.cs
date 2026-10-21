using DsGestor.Domain.Enums;

namespace DsGestor.Domain.Entities;

public class Ocorrencia
{
    public int Id { get; set; }
    public DateTime Data { get; set; }

    public string CodigoFilial { get; set; } = null!;
    public string? CodigoFilialNf { get; set; }
    public decimal NumeroNota { get; set; }
    public string SerieNota { get; set; } = null!;
    public decimal NumeroTransacaoVenda { get; set; }
    public int CodigoCliente { get; set; }
    public int? CodigoVendedor { get; set; }

    // Motorista que constava originalmente na NF
    public int? CodigoMotoristaNota { get; set; }

    // Motorista efetivamente responsável pela ocorrência
    public int CodigoMotorista { get; set; }

    public int CodigoUsuario { get; set; }
    public StatusOcorrencia Status { get; set; }

    public bool DevolucaoTotal { get; set; }
    public string? Observacao { get; set; }

    public DateTime? DataEntregaMotorista { get; set; }
    public int? CodigoUsuarioEntrega { get; set; }

    public DateTime? DataRetorno { get; set; }
    public int? CodigoUsuarioRetorno { get; set; }

    public decimal? NumeroCredito { get; set; }
    public decimal? NumeroNotaCredito { get; set; }
    public DateTime? DataCredito { get; set; }
    public int? CodigoUsuarioCredito { get; set; }

    public DateTime? DataFinalizacao { get; set; }
    public int? CodigoUsuarioFinalizacao { get; set; }

    public DateTime? DataCancelamento { get; set; }
    public int? CodigoUsuarioCancelamento { get; set; }
    public string? MotivoCancelamento { get; set; }

    public ICollection<OcorrenciaItem> Itens { get; set; }
        = new List<OcorrenciaItem>();
    public ICollection<OcorrenciaHistorico> Historicos { get; set; }
        = new List<OcorrenciaHistorico>();
    public ICollection<OcorrenciaRomaneioOcorrencia> Romaneios { get; set; }
        = new List<OcorrenciaRomaneioOcorrencia>();

    public Cliente? Cliente { get; set; }
    public Vendedor? Vendedor { get; set; }
    public Motorista? MotoristaNota { get; set; }
    public Motorista? Motorista { get; set; }
    public Usuario? Usuario { get; set; }
}
