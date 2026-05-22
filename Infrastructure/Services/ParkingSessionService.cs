using AppCore.Dto;
using AppCore.Models;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class ParkingSessionService(
    IParkingSessionRepository sessionRepository,
    IVehicleRepository vehicleRepository,
    IDiscountService discountService,
    UserManager<AppUser> userManager,
    ParkingDbContext context) : IParkingSessionService
{
    public async Task<ParkingEntryResultDto> EntryAsync(string plateNumber, string gateName)
    {
        var active = await sessionRepository.GetByLicensePlateAsync(plateNumber);
        if (active is not null)
            throw new Exception($"Pojazd {plateNumber} ma już aktywną sesję.");

        var vehicle = await vehicleRepository.FindByPlateNumber(plateNumber);
        if (vehicle is null)
        {
            vehicle = new Vehicle
            {
                LicensePlate = plateNumber.ToUpper(),
                Brand = "Nieznana",
                Color = "Nieznany"
            };
            await vehicleRepository.AddAsync(vehicle);
        }

        var session = new ParkingSession
        {
            VehicleId = vehicle.Id,
            Vehicle = vehicle,
            GateName = gateName,
            EntryTime = DateTime.UtcNow,
            IsActive = true
        };

        await sessionRepository.AddAsync(session);
        await context.SaveChangesAsync();

        return new ParkingEntryResultDto(
            session.Id,
            new VehicleDto(vehicle.Id, vehicle.LicensePlate, vehicle.Brand, vehicle.Color),
            gateName,
            session.EntryTime,
            $"Wjazd zarejestrowany. Witamy na parkingu!");
    }

    public async Task<ActiveParkingSessionDto?> GetActiveSessionAsync(string plateNumber)
    {
        var session = await sessionRepository.GetByLicensePlateAsync(plateNumber);
        if (session is null) return null;

        return new ActiveParkingSessionDto(
            session.Id,
            new VehicleDto(session.Vehicle.Id, session.Vehicle.LicensePlate, session.Vehicle.Brand, session.Vehicle.Color),
            session.GateName,
            session.EntryTime,
            DateTime.UtcNow - session.EntryTime);
    }

    public async Task<ParkingExitResultDto> ExitAsync(string plateNumber, string gateName)
    {
        return await CloseSessionAsync(plateNumber, gateName, userId: null);
    }

    public async Task<ParkingExitResultDto> PayAsync(string plateNumber, string? userId = null)
    {
        return await CloseSessionAsync(plateNumber, gateName: null, userId);
    }

    public async Task<IEnumerable<ParkingSessionHistoryDto>> GetSessionHistoryAsync(string plateNumber)
    {
        var sessions = await sessionRepository.GetParkingSessionHistoryAsync(plateNumber);

        return sessions.Select(s => new ParkingSessionHistoryDto(
            s.Id,
            new VehicleDto(s.Vehicle.Id, s.Vehicle.LicensePlate, s.Vehicle.Brand, s.Vehicle.Color),
            s.GateName,
            s.EntryTime,
            s.ExitTime,
            s.ExitTime.HasValue ? s.ExitTime.Value - s.EntryTime : null,
            s.ParkingFee,
            s.IsActive
        )).OrderByDescending(s => s.EntryTime);
    }

    private async Task<ParkingExitResultDto> CloseSessionAsync(string plateNumber, string? gateName, string? userId)
    {
        var session = await sessionRepository.GetByLicensePlateAsync(plateNumber)
            ?? throw new Exception($"Brak aktywnej sesji dla pojazdu {plateNumber}.");

        var exitTime = DateTime.UtcNow;
        var duration = exitTime - session.EntryTime;
        var freeMinutes = TariffConstants.FreeMinutes;

        var fee = CalculateFee(duration, freeMinutes);

        // Zastosuj rabaty dla zalogowanego użytkownika
        if (userId is not null)
            fee = await discountService.ApplyDiscountsAsync(userId, fee, freeMinutes);

        session.ExitTime = exitTime;
        session.ParkingFee = fee;
        session.IsActive = false;
        session.GateName = gateName ?? session.GateName;

        await sessionRepository.UpdateAsync(session.Id, session);

        // Zwiększ licznik sesji użytkownika
        if (userId is not null)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                user.TotalSessions++;
                await userManager.UpdateAsync(user);
            }
        }

        await context.SaveChangesAsync();

        return new ParkingExitResultDto(
            session.Id,
            new VehicleDto(session.Vehicle.Id, session.Vehicle.LicensePlate, session.Vehicle.Brand, session.Vehicle.Color),
            session.GateName,
            session.EntryTime,
            exitTime,
            duration,
            TimeSpan.FromMinutes(freeMinutes),
            fee,
            fee > 0,
            fee == 0 ? "Wyjazd bezpłatny." : $"Do zapłaty: {fee:F2} PLN.");
    }

    private static decimal CalculateFee(TimeSpan duration, int freeMinutes)
    {
        var billableTime = duration - TimeSpan.FromMinutes(freeMinutes);
        if (billableTime <= TimeSpan.Zero) return 0m;
        return (decimal)Math.Ceiling(billableTime.TotalHours) * TariffConstants.HourlyRate;
    }
}
