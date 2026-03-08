using AutoMapper;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Features.Payments.Commands;
using BasicBilling.API.Application.Interfaces.Repositories;
using BasicBilling.API.Application.Mappings;
using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Domain.Enums;
using Moq;

namespace BasicBilling.Tests.Handlers.Payments;

public class ProcessPaymentHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly ProcessPaymentHandler _handler;

    public ProcessPaymentHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _handler = new ProcessPaymentHandler(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidPayment_ReturnsPaymentDto()
    {
        // Arrange
        var dto = new PaymentRequestDto
        {
            ClientId = 1,
            ServiceType = ServiceType.Water,
            Period = "202501"
        };

        var bill = new Bill
        {
            Id = 10,
            ClientId = 1,
            ServiceType = ServiceType.Water,
            Period = "202501",
            Amount = 150.00m,
            Status = BillStatus.Pending
        };

        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(dto.ClientId))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Bills.GetByClientServiceAndPeriodAsync(
                dto.ClientId, dto.ServiceType, dto.Period))
            .ReturnsAsync(bill);

        _unitOfWorkMock.Setup(u => u.Payments.AddAsync(It.IsAny<Payment>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(new ProcessPaymentCommand(dto), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bill.Amount, result.Amount);
        Assert.Equal(BillStatus.Paid, bill.Status);
    }

    [Fact]
    public async Task Handle_ClientNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dto = new PaymentRequestDto
        {
            ClientId = 999,
            ServiceType = ServiceType.Water,
            Period = "202501"
        };

        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(dto.ClientId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(new ProcessPaymentCommand(dto), CancellationToken.None));

        Assert.Contains("999", exception.Message);
    }

    [Fact]
    public async Task Handle_BillNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dto = new PaymentRequestDto
        {
            ClientId = 1,
            ServiceType = ServiceType.Water,
            Period = "202501"
        };

        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(dto.ClientId))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Bills.GetByClientServiceAndPeriodAsync(
                dto.ClientId, dto.ServiceType, dto.Period))
            .ReturnsAsync((Bill?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(new ProcessPaymentCommand(dto), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_BillAlreadyPaid_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new PaymentRequestDto
        {
            ClientId = 1,
            ServiceType = ServiceType.Water,
            Period = "202501"
        };

        var paidBill = new Bill
        {
            Id = 10,
            ClientId = 1,
            ServiceType = ServiceType.Water,
            Period = "202501",
            Amount = 150.00m,
            Status = BillStatus.Paid
        };

        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(dto.ClientId))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Bills.GetByClientServiceAndPeriodAsync(
                dto.ClientId, dto.ServiceType, dto.Period))
            .ReturnsAsync(paidBill);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(new ProcessPaymentCommand(dto), CancellationToken.None));

        Assert.Contains("already been paid", exception.Message);
    }

    [Fact]
    public async Task Handle_ValidPayment_UpdatesBillStatusAndSaves()
    {
        // Arrange
        var dto = new PaymentRequestDto
        {
            ClientId = 1,
            ServiceType = ServiceType.Electricity,
            Period = "202502"
        };

        var bill = new Bill
        {
            Id = 5,
            ClientId = 1,
            ServiceType = ServiceType.Electricity,
            Period = "202502",
            Amount = 300.00m,
            Status = BillStatus.Pending
        };

        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(dto.ClientId))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Bills.GetByClientServiceAndPeriodAsync(
                dto.ClientId, dto.ServiceType, dto.Period))
            .ReturnsAsync(bill);

        _unitOfWorkMock.Setup(u => u.Payments.AddAsync(It.IsAny<Payment>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(new ProcessPaymentCommand(dto), CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Bills.Update(bill), Times.Once);
        _unitOfWorkMock.Verify(u => u.Payments.AddAsync(It.IsAny<Payment>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
