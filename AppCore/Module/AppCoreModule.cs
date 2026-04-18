using AppCore.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppCore.Module;

public static class AppCoreModule
{
    public static IServiceCollection AddAppCoreModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Rejestracja walidatorów
        services.AddValidatorsFromAssemblyContaining<ParkingGateValidator>();
        // pozostałe klasy walidujące
        
        // dodanie automatycznej walidacji
        services.AddFluentValidationAutoValidation();
        return services;
    }
}