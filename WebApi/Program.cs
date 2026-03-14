using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.Memory;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddScoped<IParkingGateRepository, InMemoryParkingGateRepository<ParkingGate>>();
        builder.Services.AddScoped<IParkingSessionRepository, InMemoryParkingSessionRepository<ParkingSession>>();
        builder.Services.AddScoped<IVehicleRepository, InMemoryVehicleRepository<Vehicle>>();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        

        

        app.Run();
    }
}