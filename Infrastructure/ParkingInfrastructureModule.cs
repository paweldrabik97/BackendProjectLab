using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.EntityFramework.Repositories;
using Infrastructure.EntityFramework.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ParkingInfrastructureModule
{
    public static IServiceCollection AddParkingEfModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Rejestracja Repozytoriów
        services.AddScoped<IParkingGateRepository, EfParkingGateRepository>();
        
        // Rejestracja UnitOfWork
        services.AddScoped<IParkingUnitOfWork, EfParkingUnitOfWork>();
        
        // Konfiguracja DbContext - pobieranie connection stringa
        services.AddDbContext<ParkingDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("ParkingDb")));
        
        // Konfiguracja Identity
        services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<ParkingDbContext>()
            .AddDefaultTokenProviders();
            
        // Rejestracja Serwisu
        services.AddScoped<IParkingGateService, ParkingGateService>();

        return services;
    }
}