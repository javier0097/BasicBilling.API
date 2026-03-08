using AutoMapper;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Features.Payments.Queries;
using BasicBilling.API.Application.Interfaces.Repositories;
using BasicBilling.API.Application.Mappings;
using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Domain.Enums;
using Moq;

namespace BasicBilling.Tests.Handlers.Payments;

public class GetPaymentHistoryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly GetPaymentHistoryHandler _handler;

    public GetPaymentHistoryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _handler = new GetPaymentHistoryHandler(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidClient_ReturnsPaymentHistory()
    {
        // Arrange
        var bill = new Bill
        {
            Id = 1,
            ClientId = 1,
            ServiceType = ServiceType.Water,
            Period = "202501",
            Amount = 100m,
            Status = BillStatus.Paid
        };

        var payments = new List<Payment>
        {
            new() { Id = 1, BillId = 1, Bill = bill, Amount = 100m, PaymentDate = DateTime.UtcNow },
            new() { Id = 2, BillId = 1, Bill = bill, Amount = 100m, PaymentDate = DateTime.UtcNow.AddDays(-30) }
        }.AsQueryable();

        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(1))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Payments.GetPaymentsByClientId(1))
            .Returns(payments);

        // Act
        var result = await _handler.Handle(new GetPaymentHistoryQuery(1), CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.All(resultList, p => Assert.True(p.Amount > 0));
    }

    [Fact]
    public async Task Handle_ClientNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(999))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(new GetPaymentHistoryQuery(999), CancellationToken.None));

        Assert.Contains("999", exception.Message);
    }

    [Fact]
    public async Task Handle_NoPayments_ReturnsEmptyCollection()
    {
        // Arrange
        var emptyPayments = new List<Payment>().AsQueryable();

        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(1))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Payments.GetPaymentsByClientId(1))
            .Returns(emptyPayments);

        // Act
        var result = await _handler.Handle(new GetPaymentHistoryQuery(1), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
