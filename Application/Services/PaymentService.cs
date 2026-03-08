using AutoMapper;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Interfaces.Repositories;
using BasicBilling.API.Application.Interfaces.Services;
using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Domain.Enums;

namespace BasicBilling.API.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaymentDto> ProcessPaymentAsync(PaymentRequestDto dto)
    {
        var clientExists = await _unitOfWork.Clients.ExistsAsync(dto.ClientId);
        if (!clientExists)
            throw new KeyNotFoundException($"Client with ID {dto.ClientId} not found.");

        var bill = await _unitOfWork.Bills.GetByClientServiceAndPeriodAsync(
            dto.ClientId, dto.ServiceType, dto.Period);

        if (bill == null)
            throw new KeyNotFoundException("No bill found for the provided client, service type, and period.");

        if (bill.Status == BillStatus.Paid)
            throw new InvalidOperationException("This bill has already been paid.");

        bill.Status = BillStatus.Paid;
        _unitOfWork.Bills.Update(bill);

        var payment = new Payment
        {
            BillId = bill.Id,
            Amount = bill.Amount,
            PaymentDate = DateTime.UtcNow
        };

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PaymentDto>(payment);
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentHistoryByClientIdAsync(int clientId)
    {
        var clientExists = await _unitOfWork.Clients.ExistsAsync(clientId);
        if (!clientExists)
            throw new KeyNotFoundException($"Client with ID {clientId} not found.");

        var payments = await _unitOfWork.Payments.GetPaymentsByClientIdAsync(clientId);

        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }
}
