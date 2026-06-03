using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DsGestor.Domain.Entities;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;

namespace DsGestor.Infrastructure.Repositories
{
    public class PromocaoClienteRepository : IPromocaoClienteRepository
    {
        private readonly DsGestorDbContext _context;
        public PromocaoClienteRepository(DsGestorDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PromocaoCliente promocaoCliente, CancellationToken ct)
        {
            await _context.PromocaoClientes.AddAsync(promocaoCliente, ct);
        }
    }
}