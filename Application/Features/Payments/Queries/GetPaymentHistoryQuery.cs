using AutoMapper;
using AutoMapper.QueryableExtensions;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Interfaces.Repositories;
using MediatR;

namespace BasicBilling.API.Application.Features.Payments.Queries;

public record GetPaymentHistoryQuery(int ClientId) : IRequest<IQueryable<PaymentDto>>;

public class GetPaymentHistoryHandler : IRequestHandler<GetPaymentHistoryQuery, IQueryable<PaymentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPaymentHistoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IQueryable<PaymentDto>> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        var clientExists = await _unitOfWork.Clients.ExistsAsync(request.ClientId);
        if (!clientExists)
            throw new KeyNotFoundException($"Client with ID {request.ClientId} not found.");

        return _unitOfWork.Payments
            .GetPaymentsByClientId(request.ClientId)
            .ProjectTo<PaymentDto>(_mapper.ConfigurationProvider);
    }
}
