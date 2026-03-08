namespace BasicBilling.API.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    IClientRepository Clients { get; }
    IBillRepository Bills { get; }
    IPaymentRepository Payments { get; }
    Task<int> SaveChangesAsync();
}
