using System.Net;
using System.Net.Http.Json;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Domain.Enums;

namespace BasicBilling.Tests.Integration;

public class BillsEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public BillsEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = await TestHelper.GetAuthenticatedClientAsync(_factory);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateBill_ValidData_ReturnsCreatedWithBill()
    {
        // Arrange
        var dto = new CreateBillDto
        {
            ClientId = 200,
            ServiceType = ServiceType.Water,
            Period = "202503",
            Amount = 175.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/bills", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var bill = await response.Content.ReadFromJsonAsync<BillDto>();
        Assert.NotNull(bill);
        Assert.Equal(200, bill.ClientId);
        Assert.Equal(175.00m, bill.Amount);
        Assert.Equal("Pending", bill.Status);
    }

    [Fact]
    public async Task CreateBill_InvalidClient_ReturnsNotFound()
    {
        // Arrange
        var dto = new CreateBillDto
        {
            ClientId = 999,
            ServiceType = ServiceType.Water,
            Period = "202501",
            Amount = 100.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/bills", dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateBill_NoToken_ReturnsUnauthorized()
    {
        // Arrange - uses a client WITHOUT token
        var unauthenticatedClient = _factory.CreateClient();
        var dto = new CreateBillDto
        {
            ClientId = 100,
            ServiceType = ServiceType.Water,
            Period = "202503",
            Amount = 100.00m
        };

        // Act
        var response = await unauthenticatedClient.PostAsJsonAsync("/api/bills", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateBill_DuplicateBill_ReturnsConflict()
    {
        // Arrange
        var dto = new CreateBillDto
        {
            ClientId = 100,
            ServiceType = ServiceType.Electricity,
            Period = "202506",
            Amount = 250.00m
        };

        // Act - create the bill twice
        var firstResponse = await _client.PostAsJsonAsync("/api/bills", dto);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondResponse = await _client.PostAsJsonAsync("/api/bills", dto);

        // Assert - second attempt should return 409 Conflict
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }
}
