using AppCore.Dto;
using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.Memory;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddOpenApi();

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<IParkingGateRepository, MemoryParkingGateRepository<ParkingGate>>();
        builder.Services.AddSingleton<IParkingSessionRepository, MemoryParkingSessionRepository<ParkingSession>>();
        builder.Services.AddSingleton<IVehicleRepository, MemoryVehicleRepository<Vehicle>>();
        builder.Services.AddSingleton<IParkingUnitOfWork, MemoryParkingUnitOfWork>();
        builder.Services.AddSingleton<IParkingGateService, MemoryParkingGateService>();

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

        app.MapControllers();

        //

        app.Run();
    }
}