using BasicBilling.API.Application.DTOs;

namespace BasicBilling.API.Application.Interfaces.Services;

public interface IPaymentService
{
    Task<PaymentDto> ProcessPaymentAsync(PaymentRequestDto dto);
    Task<IEnumerable<PaymentDto>> GetPaymentHistoryByClientIdAsync(int clientId);
}
