using System.Net;
using System.Net.Http.Json;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Domain.Enums;

namespace BasicBilling.Tests.Integration;

public class PaymentsEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public PaymentsEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = await TestHelper.GetAuthenticatedClientAsync(_factory);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ProcessPayment_ValidData_ReturnsOkWithPayment()
    {
        // Arrange
        var dto = new PaymentRequestDto
        {
            ClientId = 200,
            ServiceType = ServiceType.Sewer,
            Period = "202602"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/payments", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payment = await response.Content.ReadFromJsonAsync<PaymentDto>();
        Assert.NotNull(payment);
        Assert.True(payment.Amount > 0);
    }

    [Fact]
    public async Task ProcessPayment_AlreadyPaid_ReturnsConflict()
    {
        // Arrange
        var dto = new PaymentRequestDto
        {
            ClientId = 100,
            ServiceType = ServiceType.Water,
            Period = "202602"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/payments", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task ProcessPayment_ClientNotFound_ReturnsNotFound()
    {
        // Arrange
        var dto = new PaymentRequestDto
        {
            ClientId = 999,
            ServiceType = ServiceType.Water,
            Period = "202501"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/payments", dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ProcessPayment_BillNotFound_ReturnsNotFound()
    {
        // Arrange
        var dto = new PaymentRequestDto
        {
            ClientId = 100,
            ServiceType = ServiceType.Sewer,
            Period = "209912"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/payments", dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
