using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DsGestor.Domain.Entities;

public class LancamentoDet
{
    public int Id { get; set; }
    public int CodigoLancamento { get; set; }
    public int CodigoProduto { get; set; }
    public Decimal PrecoCustoFin { get; set; }
    public Decimal CodigoIcmTab { get; set; }
    public Decimal PrecoVenda { get; set; } 
    public Decimal MargemIdeal { get; set; }
    public Decimal Quantidade { get; set; }
    public Decimal PrecoUnitario { get; set; }
    public Decimal PercDesconto { get; set; }
    public Decimal Margem { get; set; }
    public Decimal QuantidadeAut { get; set; }
    public Decimal PrecoUnitarioAut { get; set;}
    public Decimal PercDescontoAut { get; set;}
    public Decimal MargemAut { get; set; }
    public Decimal PrecoMinimo { get; set; }
    public Lancamento? Lancamento { get; set; }
    public Produto? Produto { get; set; }
  
}
