using Microsoft.EntityFrameworkCore;
using BasicBilling.API.Application.Interfaces.Repositories;
using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Infrastructure.Data;

namespace BasicBilling.API.Infrastructure.Repositories;

public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(BillingDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByClientIdAsync(int clientId)
    {
        return await _dbSet
            .Include(p => p.Bill)
            .Where(p => p.Bill.ClientId == clientId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }
}
