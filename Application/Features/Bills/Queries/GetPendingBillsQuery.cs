using AutoMapper;
using AutoMapper.QueryableExtensions;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Interfaces.Repositories;
using MediatR;

namespace BasicBilling.API.Application.Features.Bills.Queries;

public record GetPendingBillsQuery(int ClientId) : IRequest<IQueryable<BillDto>>;

public class GetPendingBillsHandler : IRequestHandler<GetPendingBillsQuery, IQueryable<BillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPendingBillsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IQueryable<BillDto>> Handle(GetPendingBillsQuery request, CancellationToken cancellationToken)
    {
        var clientExists = await _unitOfWork.Clients.ExistsAsync(request.ClientId);
        if (!clientExists)
            throw new KeyNotFoundException($"Client with ID {request.ClientId} not found.");

        return _unitOfWork.Bills
            .GetPendingBillsByClientId(request.ClientId)
            .ProjectTo<BillDto>(_mapper.ConfigurationProvider);
    }
}
