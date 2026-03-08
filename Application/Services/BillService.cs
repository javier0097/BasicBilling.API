using AutoMapper;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Interfaces.Repositories;
using BasicBilling.API.Application.Interfaces.Services;
using BasicBilling.API.Domain.Entities;

namespace BasicBilling.API.Application.Services;

public class BillService : IBillService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BillService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BillDto> CreateBillAsync(CreateBillDto dto)
    {
        var clientExists = await _unitOfWork.Clients.ExistsAsync(dto.ClientId);
        if (!clientExists)
            throw new KeyNotFoundException($"Client with ID {dto.ClientId} not found.");

        var bill = _mapper.Map<Bill>(dto);

        await _unitOfWork.Bills.AddAsync(bill);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BillDto>(bill);
    }

    public async Task<IEnumerable<BillDto>> GetPendingBillsByClientIdAsync(int clientId)
    {
        var clientExists = await _unitOfWork.Clients.ExistsAsync(clientId);
        if (!clientExists)
            throw new KeyNotFoundException($"Client with ID {clientId} not found.");

        var bills = await _unitOfWork.Bills.GetPendingBillsByClientIdAsync(clientId);

        return _mapper.Map<IEnumerable<BillDto>>(bills);
    }
}
