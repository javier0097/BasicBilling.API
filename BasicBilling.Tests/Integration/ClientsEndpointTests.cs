using System.Net;
using System.Net.Http.Json;
using BasicBilling.API.Application.DTOs;

namespace BasicBilling.Tests.Integration;

public class ClientsEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public ClientsEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = await TestHelper.GetAuthenticatedClientAsync(_factory);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetPendingBills_ValidClient_ReturnsOkWithBills()
    {
        // Act
        var response = await _client.GetAsync("/api/clients/100/pending-bills");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var bills = await response.Content.ReadFromJsonAsync<List<BillDto>>();
        Assert.NotNull(bills);
        Assert.Equal(5, bills.Count);
        Assert.All(bills, b => Assert.Equal("Pending", b.Status));
    }

    [Fact]
    public async Task GetPendingBills_InvalidClient_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/clients/999/pending-bills");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPaymentHistory_ValidClient_ReturnsOkWithPayments()
    {
        // Act
        var response = await _client.GetAsync("/api/clients/100/payment-history");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payments = await response.Content.ReadFromJsonAsync<List<PaymentDto>>();
        Assert.NotNull(payments);
        Assert.True(payments.Count >= 1);
    }

    [Fact]
    public async Task GetPaymentHistory_InvalidClient_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/clients/999/payment-history");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPendingBills_NoToken_ReturnsUnauthorized()
    {
        // Arrange - uses a client WITHOUT token
        var unauthenticatedClient = _factory.CreateClient();

        // Act
        var response = await unauthenticatedClient.GetAsync("/api/clients/100/pending-bills");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
