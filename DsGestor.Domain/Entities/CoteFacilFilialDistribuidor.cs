namespace DsGestor.Domain.Entities;

public class CoteFacilFilialDistribuidor
{
    public string CodigoFilial { get; set; } = string.Empty;
    public string? RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string? Cidade { get; set; }
    public string? Uf { get; set; }
}