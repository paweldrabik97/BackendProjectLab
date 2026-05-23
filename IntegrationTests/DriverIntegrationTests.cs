using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using IntegrationTests.TestHelpers;
using Xunit;

namespace IntegrationTests;

[Collection("Shared API Collection")]
public class DriverIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public DriverIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    record LoginRequest(string Email, string Password);
    record RegisterVehicleDto(string PlateNumber, string Brand);

    private async Task AuthenticateAsSeededAdminAsync()
    {
        var login = new LoginRequest("admin@app.pl", "Admin@123!");
        var resp = await _client.PostAsJsonAsync("/api/auth/login", login);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
        var token = json.GetProperty("accessToken").GetString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task RegisterVehicle_And_GetVehicles_Works()
    {
        // Arrange
        await AuthenticateAsSeededAdminAsync();
        var dto = new RegisterVehicleDto("TEST-123", "Toyota");

        // Act - register
        var registerResp = await _client.PostAsJsonAsync("/api/driver/vehicles", dto);

        // Assert - created
        registerResp.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act - list vehicles
        var listResp = await _client.GetAsync("/api/driver/vehicles");
        listResp.EnsureSuccessStatusCode();
        var listJson = await listResp.Content.ReadFromJsonAsync<IEnumerable<JsonElement>>();

        // Assert - contains the registered plate
        listJson.Should().NotBeNull();
        listJson.Should().Contain(x => x.GetProperty("plateNumber").GetString() == "TEST-123");
    }
}