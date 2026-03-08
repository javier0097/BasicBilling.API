using AutoMapper;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Interfaces.Repositories;
using MediatR;

namespace BasicBilling.API.Application.Features.Bills.Queries;

public record GetPendingBillsQuery(int ClientId) : IRequest<IEnumerable<BillDto>>;

public class GetPendingBillsHandler : IRequestHandler<GetPendingBillsQuery, IEnumerable<BillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPendingBillsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BillDto>> Handle(GetPendingBillsQuery request, CancellationToken cancellationToken)
    {
        var clientExists = await _unitOfWork.Clients.ExistsAsync(request.ClientId);
        if (!clientExists)
            throw new KeyNotFoundException($"Client with ID {request.ClientId} not found.");

        var bills = await _unitOfWork.Bills.GetPendingBillsByClientIdAsync(request.ClientId);

        return _mapper.Map<IEnumerable<BillDto>>(bills);
    }
}
