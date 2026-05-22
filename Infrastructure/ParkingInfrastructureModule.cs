using AppCore.Models;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.EntityFramework.Repositories;
using Infrastructure.EntityFramework.UnitOfWork;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class ParkingInfrastructureModule
{
    public static IServiceCollection AddParkingEfModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Rejestracja Repozytoriów
        services.AddScoped<IParkingGateRepository, EfParkingGateRepository>();
        services.AddScoped<IRegisteredVehicleRepository, EfRegisteredVehicleRepository>();
        services.AddScoped<IDriverDiscountRepository, EfDriverDiscountRepository>();
        services.AddScoped<IWalletRepository, EfWalletRepository>();

        // Rejestracja UnitOfWork
        services.AddScoped<IParkingUnitOfWork, EfParkingUnitOfWork>();

        // Konfiguracja DbContext - pobieranie connection stringa
        services.AddDbContext<ParkingDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("ParkingDb")));

        //Rejestracja Seedera
        services.AddScoped<IDataSeeder, IdentityDbSeeder>();

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

        // Rejestracja Serwisów
        services.AddScoped<IParkingGateService, ParkingGateService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDiscountService, DiscountService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IParkingSessionService, ParkingSessionService>();

        return services;
    }

    public static IServiceCollection AddJwt(this IServiceCollection services, JwtSettings jwtOptions)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = jwtOptions.GetSymmetricKey(),
                    ClockSkew = TimeSpan.Zero // brak tolerancji czasu
                };
            }
            );
        services.AddAuthorization(options =>
        {
            // Polityki oparte o role
            // metoda RequireRole akceptuje dowolną liczbę parametrów typu string
            options.AddPolicy(AppPolicies.AdminOnly.ToString(), policy =>
                policy.RequireRole(UserRole.Administrator.ToString()));


            // dodaj polityki dla pozostałych stałych
            // np. która wymaga użytkownika z jedną z ról: Administrator, Student

            // Polityka złożona — wymaga roli i aktywnego konta
            // Zakładamy, że w AppPolicies jest stała ActiveUser
            options.AddPolicy(AppPolicies.ActiveUser.ToString(), policy =>
                policy
                    .RequireAuthenticatedUser()
                    .RequireClaim("status", SystemUserStatus.Active.ToString()));

            // Polityka oparta o dział
            // gdydy była stała SalesDepartment w AppPolicies
            // to dostęp mają użytkownicy, którzy w tokenie mają roszczenia department z wartością Sales 
            options.AddPolicy(AppPolicies.SalesDepartment.ToString(), policy =>
                policy.RequireClaim("department", "Sales"));

            // Domyślna polityka — każdy zalogowany użytkownik
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Polityka fallback — stosowana gdy brak atrybutu [Authorize]
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
        return services;
    }
}