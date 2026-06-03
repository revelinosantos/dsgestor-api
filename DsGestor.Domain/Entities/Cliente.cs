
namespace DsGestor.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string RazaoSocial { get; set; } = null!;
    public string? NomeFantasia { get; set; }
    public string CnpjCpf { get; set; } = null!;
    public string? Endereco { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Fone { get; set; }
    public Decimal? LimiteCredito { get;set; }
    public DateTime? DataCadastro { get; set; }
    public DateTime? DataUltimaCompra { get; set;}
    public string CodigoFilial { get; set; } = null!;
    public int CodigoVendedor { get; set; }
    public string NomeVendedor { get; set; } = null!;
    public int CodigoSupervisor { get; set; }
    public string NomeSupervisor { get; set; } = null!;
    public int NumRegiao { get; set; }
    public string NomeRegiao { get; set; } = null!;
        
}
