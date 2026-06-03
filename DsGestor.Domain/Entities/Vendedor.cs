using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DsGestor.Domain.Entities
{
    public class Vendedor
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public DateTime? DataTermino { get; set; }
        public int CodigoSupervisor { get; set; }
        public Supervisor? Supervisor { get; set; }
        
    }
}