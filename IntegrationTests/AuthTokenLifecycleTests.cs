using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using IntegrationTests.TestHelpers;
using Xunit;

namespace IntegrationTests;

[Collection("IntegrationTests")]
public class AuthTokenLifecycleTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthTokenLifecycleTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    record LoginRequest(string Email, string Password);

    private async Task<string> LoginAndGetAccessTokenAsync(string email = "admin@app.pl", string password = "Admin@123!")
    {
        var login = new LoginRequest(email, password);
        var resp = await _client.PostAsJsonAsync("/api/auth/login", login);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("accessToken").GetString()!;
    }

    private async Task<string> LoginAndGetRefreshTokenAsync()
    {
        var login = new LoginRequest("admin@app.pl", "Admin@123!");
        var resp = await _client.PostAsJsonAsync("/api/auth/login", login);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("refreshToken").GetString()!;
    }

    [Fact]
    public async Task Me_Returns_UserInfo_When_Authorized()
    {
        // Arrange
        var token = await LoginAndGetAccessTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var resp = await _client.GetAsync("/api/auth/me");

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await resp.Content.ReadFromJsonAsync<JsonElement>();
        user.GetProperty("email").GetString().Should().Be("admin@app.pl");
        user.GetProperty("roles").EnumerateArray().Should().NotBeEmpty();
    }

    [Fact]
    public async Task Revoke_Prevents_RefreshToken_From_Being_Used()
    {
        // Arrange - login
        var login = new LoginRequest("admin@app.pl", "Admin@123!");
        var loginResp = await _client.PostAsJsonAsync("/api/auth/login", login);
        loginResp.EnsureSuccessStatusCode();
        var loginJson = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = loginJson.GetProperty("accessToken").GetString();
        var refreshToken = loginJson.GetProperty("refreshToken").GetString();

        // Revoke (requires Authorization)
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var revokeResp = await _client.PostAsJsonAsync("/api/auth/revoke", refreshToken);
        revokeResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Attempt to refresh with revoked token
        var refreshBody = new { AccessToken = accessToken, RefreshToken = refreshToken };
        var refreshResp = await _client.PostAsJsonAsync("/api/auth/refresh", refreshBody);

        // Service throws on invalid refresh token -> controller returns non-OK
        refreshResp.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }
}