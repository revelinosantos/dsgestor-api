using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DsGestor.Domain.Entities;

namespace DsGestor.Domain.Repositories
{
    public interface IPromocaoClienteRepository
    {
         Task AddAsync(PromocaoCliente promocaoCliente, CancellationToken ct);
    }
}