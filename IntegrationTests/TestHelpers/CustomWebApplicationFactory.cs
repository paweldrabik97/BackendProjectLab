using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.EntityFramework.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IntegrationTests.TestHelpers;

public class CustomWebApplicationFactory : WebApplicationFactory<WebApi.Program>, IAsyncDisposable
{
    private DbConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 1. Konfiguracja środowiska i ustawień JWT
        builder.UseEnvironment("Test"); // Wymuszamy środowisko Testowe
        
        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:Secret"] = "TestSecretKeySuperLongForTests_ChangeInProd",
                ["Jwt:ExpirationInMinutes"] = "60",
                ["Jwt:RefreshTokenDays"] = "30"
            });
        });

        // 2. Podmiana bazy danych
        builder.ConfigureServices(services =>
        {
            // Usuń oryginalny DbContext z WebApi
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ParkingDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Tworzymy unikalne połączenie w pamięci
            _connection = new SqliteConnection($"Data Source={Guid.NewGuid()};Mode=Memory;Cache=Shared");
            _connection.Open(); // Otwieramy połączenie, by baza żyła

            // Zarejestruj testowy DbContext
            services.AddDbContext<ParkingDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ParkingDbContext>();
        
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        // Seedowanie
        var seeder = scope.ServiceProvider.GetService<AppCore.Services.IDataSeeder>();
        if (seeder is not null)
        {
            seeder.SeedAsync().GetAwaiter().GetResult();
        }

        return host;
    }

    public new async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
        await base.DisposeAsync();
    }
}