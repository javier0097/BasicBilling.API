using Microsoft.EntityFrameworkCore;
using BasicBilling.API.Application.Interfaces.Repositories;
using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Domain.Enums;
using BasicBilling.API.Infrastructure.Data;

namespace BasicBilling.API.Infrastructure.Repositories;

public class BillRepository : BaseRepository<Bill>, IBillRepository
{
    public BillRepository(BillingDbContext context) : base(context)
    {
    }

    public IQueryable<Bill> GetPendingBillsByClientId(int clientId)
    {
        return _dbSet
            .Where(b => b.ClientId == clientId && b.Status == BillStatus.Pending);
    }

    public async Task<Bill?> GetByClientServiceAndPeriodAsync(int clientId, ServiceType serviceType, string period)
    {
        return await _dbSet
            .FirstOrDefaultAsync(b => b.ClientId == clientId && b.ServiceType == serviceType && b.Period == period);
    }
}
