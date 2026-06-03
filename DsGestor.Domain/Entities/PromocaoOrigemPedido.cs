using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DsGestor.Domain.Entities
{
    public class PromocaoOrigemPedido
    {
        public int Id { get; set; }
        public string OrigemPed { get; set; } = "T";
    }
}