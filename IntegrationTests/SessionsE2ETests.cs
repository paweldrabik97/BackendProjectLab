using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using IntegrationTests.TestHelpers;
using Xunit;

namespace IntegrationTests;

[Collection("IntegrationTests")]
public class SessionsE2ETests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SessionsE2ETests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    record EntryDto(string PlateNumber, string GateName);
    record PayResponseDto(decimal Fee, Guid SessionId);

    [Fact]
    public async Task Entry_Status_And_Pay_Work_For_Anonymous_User()
    {
        // Arrange
        var plate = $"INT-{Guid.NewGuid():N}".Substring(0, 10);
        var entry = new EntryDto(plate, "MainGate");

        // Act - entry
        var entryResp = await _client.PostAsJsonAsync("/api/sessions/entry", entry);

        // Assert
        entryResp.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act - get status (anonymous)
        var statusResp = await _client.GetAsync($"/api/sessions/{plate}/status");
        statusResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var statusJson = await statusResp.Content.ReadFromJsonAsync<JsonElement>();
        statusJson.GetProperty("plateNumber").GetString().Should().Be(plate);

        // Act - pay (anonymous)
        var payResp = await _client.PostAsync($"/api/sessions/{plate}/pay", null);

        // Assert - should return 200 and include fee/session id
        payResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var payJson = await payResp.Content.ReadFromJsonAsync<JsonElement>();
        payJson.TryGetProperty("fee", out _).Should().BeTrue();
        payJson.TryGetProperty("sessionId", out _).Should().BeTrue();
    }
}