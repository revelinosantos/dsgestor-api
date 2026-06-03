namespace DsGestor.Domain.Entities;

public class Produto
{
    public int Id { get; set; }     
    public string CodigoFilial { get; set; } = null!;  
    public int CodigoRegiao { get; set; }   
    public string NomeRegiao { get; set; } = null!;
    public string Estado { get; set; } = null!;     
    public string Descricao { get; set; } = null!;
    public int CodigoFornecedor { get; set; }               // NUMBER(6,0)
    public string Fornecedor { get; set; } = null!;
    public int CodigoDepto { get; set; }        
    public string DescricaoDepto { get; set; } = null!;
    public string Unidade { get; set; } = null!;
    public DateTime? DataUltimaEntrada { get; set; }
    public Decimal? CustoFinanceiro { get; set; }    
    public Decimal? CodigoIcmTab { get; set; } 
    public Decimal? Estoque { get; set; }          
    public Decimal? PrecoTabela { get; set; }   
    public Decimal? PrecoVenda { get; set; }       
    public Decimal? MargemIdeal { get; set; }      
    public Decimal? PrecoMinimo { get; set; }
}
