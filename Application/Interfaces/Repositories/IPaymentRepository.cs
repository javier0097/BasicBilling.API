using BasicBilling.API.Domain.Entities;

namespace BasicBilling.API.Application.Interfaces.Repositories;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<IEnumerable<Payment>> GetPaymentsByClientIdAsync(int clientId);
}
