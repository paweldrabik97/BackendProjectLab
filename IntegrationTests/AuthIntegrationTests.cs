using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using IntegrationTests.TestHelpers;
using Xunit;

namespace IntegrationTests;

[Collection("Shared API Collection")]
public class AuthIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    record LoginRequest(string Email, string Password);
    record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);

    [Fact]
    public async Task Login_Returns_Access_And_Refresh_Token()
    {
        // Arrange 
        var login = new LoginRequest("admin@app.pl", "Admin@123!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", login);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.TryGetProperty("accessToken", out var at).Should().BeTrue();
        body.TryGetProperty("refreshToken", out var rt).Should().BeTrue();
        at.GetString().Should().NotBeNullOrEmpty();
        rt.GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Refresh_Returns_New_AccessToken()
    {
        // Arrange - login first
        var login = new LoginRequest("admin@app.pl", "Admin@123!");
        var loginResp = await _client.PostAsJsonAsync("/api/auth/login", login);
        loginResp.EnsureSuccessStatusCode();
        var loginJson = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = loginJson.GetProperty("accessToken").GetString();
        var refreshToken = loginJson.GetProperty("refreshToken").GetString();

        // Build refresh request
        var refreshBody = new { AccessToken = accessToken, RefreshToken = refreshToken };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("accessToken").GetString().Should().NotBeNullOrEmpty();
        body.GetProperty("refreshToken").GetString().Should().NotBeNullOrEmpty();
    }
}