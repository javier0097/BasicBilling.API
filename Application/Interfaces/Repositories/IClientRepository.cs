using BasicBilling.API.Domain.Entities;

namespace BasicBilling.API.Application.Interfaces.Repositories;

public interface IClientRepository : IBaseRepository<Client>
{
    Task<bool> ExistsAsync(int clientId);
}
