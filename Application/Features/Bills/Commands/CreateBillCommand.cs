using AutoMapper;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Interfaces.Repositories;
using BasicBilling.API.Domain.Entities;
using MediatR;

namespace BasicBilling.API.Application.Features.Bills.Commands;

public record CreateBillCommand(CreateBillDto Dto) : IRequest<BillDto>;

public class CreateBillHandler : IRequestHandler<CreateBillCommand, BillDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBillHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BillDto> Handle(CreateBillCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _unitOfWork.Clients.ExistsAsync(request.Dto.ClientId);
        if (!clientExists)
            throw new KeyNotFoundException($"Client with ID {request.Dto.ClientId} not found.");

        var bill = _mapper.Map<Bill>(request.Dto);

        await _unitOfWork.Bills.AddAsync(bill);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BillDto>(bill);
    }
}
