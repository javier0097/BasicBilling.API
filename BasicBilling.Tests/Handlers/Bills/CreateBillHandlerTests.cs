using AutoMapper;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Features.Bills.Commands;
using BasicBilling.API.Application.Interfaces.Repositories;
using BasicBilling.API.Application.Mappings;
using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Domain.Enums;
using Moq;

namespace BasicBilling.Tests.Handlers.Bills;

public class CreateBillHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly CreateBillHandler _handler;

    public CreateBillHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _handler = new CreateBillHandler(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidClient_ReturnsBillDto()
    {
        // Arrange
        var dto = new CreateBillDto
        {
            ClientId = 1,
            ServiceType = ServiceType.Water,
            Period = "202501",
            Amount = 150.00m
        };

        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(dto.ClientId))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Bills.AddAsync(It.IsAny<Bill>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(new CreateBillCommand(dto), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.ClientId, result.ClientId);
        Assert.Equal(dto.Amount, result.Amount);
        Assert.Equal("Water", result.ServiceType);
        Assert.Equal("Pending", result.Status);
    }

    [Fact]
    public async Task Handle_ClientNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dto = new CreateBillDto
        {
            ClientId = 999,
            ServiceType = ServiceType.Water,
            Period = "202501",
            Amount = 100.00m
        };

        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(dto.ClientId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(new CreateBillCommand(dto), CancellationToken.None));

        Assert.Contains("999", exception.Message);
    }

    [Fact]
    public async Task Handle_ValidClient_CallsAddAsyncAndSaveChanges()
    {
        // Arrange
        var dto = new CreateBillDto
        {
            ClientId = 1,
            ServiceType = ServiceType.Electricity,
            Period = "202503",
            Amount = 200.00m
        };

        _unitOfWorkMock.Setup(u => u.Clients.ExistsAsync(dto.ClientId))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Bills.AddAsync(It.IsAny<Bill>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(new CreateBillCommand(dto), CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Bills.AddAsync(It.IsAny<Bill>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
