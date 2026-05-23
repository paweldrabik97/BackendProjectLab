using IntegrationTests.TestHelpers;
using Xunit;

namespace IntegrationTests;

[CollectionDefinition("Shared API Collection")]
public class SharedApiCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}