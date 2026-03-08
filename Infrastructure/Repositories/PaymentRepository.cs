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

    public IQueryable<Payment> GetPaymentsByClientId(int clientId)
    {
        return _dbSet
            .Include(p => p.Bill)
            .Where(p => p.Bill.ClientId == clientId);
    }
}
