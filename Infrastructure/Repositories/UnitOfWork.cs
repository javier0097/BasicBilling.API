using BasicBilling.API.Application.Interfaces.Repositories;
using BasicBilling.API.Infrastructure.Data;

namespace BasicBilling.API.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BillingDbContext _context;

    public IClientRepository Clients { get; }
    public IBillRepository Bills { get; }
    public IPaymentRepository Payments { get; }

    public UnitOfWork(BillingDbContext context)
    {
        _context = context;
        Clients = new ClientRepository(context);
        Bills = new BillRepository(context);
        Payments = new PaymentRepository(context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
