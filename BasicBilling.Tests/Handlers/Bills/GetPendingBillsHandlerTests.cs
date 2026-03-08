using AutoMapper;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Features.Bills.Queries;
using BasicBilling.API.Application.Interfaces.Repositories;
using BasicBilling.API.Application.Mappings;
using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Domain.Enums;
using Moq;

namespace BasicBilling.Tests.Handlers.Bills;

public class GetPendingBillsHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly GetPendingBillsHandler _handler;

    public GetPendingBillsHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _handler = new GetPendingBillsHandler(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidClient_ReturnsPendingBills()
    {
        // Arrange
        var bills = new List<Bill>
        {
            new() { Id = 1, ClientId = 1, ServiceType = ServiceType.Water, Period = "202501", Amount = 100m, Status = BillStatus.Pending },
            new() { Id = 2, ClientId = 1, ServiceType = ServiceType.Electricity, Period = "202501", Amount = 200m, Status = BillStatus.Pending }
        }.AsQueryable();

        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(1))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Bills.GetPendingBillsByClientId(1))
            .Returns(bills);

        // Act
        var result = await _handler.Handle(new GetPendingBillsQuery(1), CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.All(resultList, b => Assert.Equal("Pending", b.Status));
    }

    [Fact]
    public async Task Handle_ClientNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(999))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(new GetPendingBillsQuery(999), CancellationToken.None));

        Assert.Contains("999", exception.Message);
    }

    [Fact]
    public async Task Handle_NoPendingBills_ReturnsEmptyCollection()
    {
        // Arrange
        var emptyBills = new List<Bill>().AsQueryable();

        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(1))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Bills.GetPendingBillsByClientId(1))
            .Returns(emptyBills);

        // Act
        var result = await _handler.Handle(new GetPendingBillsQuery(1), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
