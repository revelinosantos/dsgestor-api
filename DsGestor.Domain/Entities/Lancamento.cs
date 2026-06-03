using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DsGestor.Domain.Enums;

namespace DsGestor.Domain.Entities;
public class Lancamento
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public string CodigoFilial { get; set; } = null!;
    public int NumRegiao { get; set; }
    public int CodigoVendedor { get; set; }
    public int CodigoCliente { get; set; }
    public int CodigoUsuario { get; set; }
    public int? CodigoUsuarioAut { get; set; }
    public DateTime? DataAutorizacao { get; set; }
    public int? CodigoPromocao { get; set; }
    public StatusLancamento Status { get; set; }
    public string? Observacao { get; set; }
    public ICollection<LancamentoDet> Itens { get; set; } = new List<LancamentoDet>();
    public Cliente? Cliente { get; set; }
    public Vendedor? Vendedor { get; set; }
    public Usuario? Usuario { get; set; }
    public Usuario? UsuarioAut { get; set; }
}
