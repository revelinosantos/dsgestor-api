using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;

namespace DsGestor.Infrastructure.Repositories
{
    public class PromocaoOrigemPedidoRepository : IPromocaoOrigemPedidoRepository
    {
        private readonly DsGestorDbContext _context;
        public PromocaoOrigemPedidoRepository(DsGestorDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PromocaoOrigemPedido promocaoOrigemPedido, CancellationToken ct)
        {
            await _context.PromocaoOrigemPedidos.AddAsync(promocaoOrigemPedido, ct);
        }
    }
}