using Microsoft.EntityFrameworkCore;
using BasicBilling.API.Application.Interfaces.Repositories;
using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Infrastructure.Data;

namespace BasicBilling.API.Infrastructure.Repositories;

public class ClientRepository : BaseRepository<Client>, IClientRepository
{
    public ClientRepository(BillingDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(int clientId)
    {
        return await _dbSet.AnyAsync(c => c.Id == clientId);
    }
}
