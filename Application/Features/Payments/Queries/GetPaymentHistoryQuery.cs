using AutoMapper;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Interfaces.Repositories;
using MediatR;

namespace BasicBilling.API.Application.Features.Payments.Queries;

public record GetPaymentHistoryQuery(int ClientId) : IRequest<IEnumerable<PaymentDto>>;

public class GetPaymentHistoryHandler : IRequestHandler<GetPaymentHistoryQuery, IEnumerable<PaymentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPaymentHistoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentDto>> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        var clientExists = await _unitOfWork.Clients.ExistsAsync(request.ClientId);
        if (!clientExists)
            throw new KeyNotFoundException($"Client with ID {request.ClientId} not found.");

        var payments = await _unitOfWork.Payments.GetPaymentsByClientIdAsync(request.ClientId);

        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }
}
