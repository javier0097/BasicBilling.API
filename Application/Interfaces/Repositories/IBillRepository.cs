using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Domain.Enums;

namespace BasicBilling.API.Application.Interfaces.Repositories;

public interface IBillRepository : IBaseRepository<Bill>
{
    IQueryable<Bill> GetPendingBillsByClientId(int clientId);
    Task<Bill?> GetByClientServiceAndPeriodAsync(int clientId, ServiceType serviceType, string period);
}
