using AutoMapper;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Interfaces.Repositories;
using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Domain.Enums;
using MediatR;

namespace BasicBilling.API.Application.Features.Payments.Commands;

public record ProcessPaymentCommand(PaymentRequestDto Dto) : IRequest<PaymentDto>;

public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, PaymentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProcessPaymentHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _unitOfWork.Clients.ExistsAsync(request.Dto.ClientId);
        if (!clientExists)
            throw new KeyNotFoundException($"Client with ID {request.Dto.ClientId} not found.");

        var bill = await _unitOfWork.Bills.GetByClientServiceAndPeriodAsync(
            request.Dto.ClientId, request.Dto.ServiceType, request.Dto.Period);

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
}
