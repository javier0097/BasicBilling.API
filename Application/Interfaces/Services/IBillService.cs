using BasicBilling.API.Application.DTOs;

namespace BasicBilling.API.Application.Interfaces.Services;

public interface IBillService
{
    Task<BillDto> CreateBillAsync(CreateBillDto dto);
    Task<IEnumerable<BillDto>> GetPendingBillsByClientIdAsync(int clientId);
}
