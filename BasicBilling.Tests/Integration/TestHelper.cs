using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BasicBilling.Tests.Integration;

public static class TestHelper
{
    public static async Task<HttpClient> GetAuthenticatedClientAsync(CustomWebApplicationFactory factory)
    {
        var client = factory.CreateClient();

        var response = await client.PostAsync("/api/auth/token", null);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result!.Token);

        return client;
    }

    private record TokenResponse(string Token);
}
