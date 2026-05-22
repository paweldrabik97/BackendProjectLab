using AppCore.Dto;
using AppCore.Models;
using AppCore.Module;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure;
using Infrastructure.EntityFramework.Context;
using Infrastructure.Memory;
using Microsoft.EntityFrameworkCore;
using WebApi.Handlers;

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
        builder.Services.AddSingleton<IParkingSessionRepository, MemoryParkingSessionRepository<ParkingSession>>();
        builder.Services.AddSingleton<IVehicleRepository, MemoryVehicleRepository<Vehicle>>();
        builder.Services.AddParkingEfModule(builder.Configuration);
        builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
        builder.Services.AddSingleton<JwtSettings>();
        builder.Services.AddJwt(new JwtSettings(builder.Configuration));
        builder.Services.AddProblemDetails();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        
        builder.Services.AddAppCoreModule(builder.Configuration);

        var app = builder.Build();

        // Automatyczne migracje + seeding przy starcie
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ParkingDbContext>();
            db.Database.Migrate();

            var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
            seeder.SeedAsync();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        if (!app.Environment.IsDevelopment())
            app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseExceptionHandler();

        app.MapControllers();

        //

        app.Run();
    }
}