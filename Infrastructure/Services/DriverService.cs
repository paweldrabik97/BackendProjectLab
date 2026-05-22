using AppCore.Dto;
using AppCore.Models;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.EntityFramework.Context;

namespace Infrastructure.Services;

public class DriverService(
    IRegisteredVehicleRepository vehicleRepository,
    IParkingSessionRepository sessionRepository,
    IDiscountService discountService,
    ParkingDbContext context) : IDriverService
{
    public async Task<RegisteredVehicleDto> RegisterVehicleAsync(string userId, RegisterVehicleDto dto)
    {
        var existing = await vehicleRepository.GetByUserIdAndPlateAsync(userId, dto.PlateNumber);
        if (existing is not null)
            throw new Exception($"Pojazd {dto.PlateNumber} jest już zarejestrowany na tym koncie.");

        var vehicle = new RegisteredVehicle
        {
            UserId = userId,
            PlateNumber = dto.PlateNumber.ToUpper(),
            Brand = dto.Brand,
            RegisteredAt = DateTime.UtcNow
        };

        await vehicleRepository.AddAsync(vehicle);
        await context.SaveChangesAsync();

        // Przyznaj bonus rejestracyjny przy pierwszym pojeździe
        var userVehicles = await vehicleRepository.GetByUserIdAsync(userId);
        if (userVehicles.Count() == 1)
            await discountService.GrantRegistrationBonusAsync(userId);

        return ToDto(vehicle);
    }

    public async Task<IEnumerable<RegisteredVehicleDto>> GetVehiclesAsync(string userId)
    {
        var vehicles = await vehicleRepository.GetByUserIdAsync(userId);
        return vehicles.Select(ToDto);
    }

    public async Task<IEnumerable<ParkingSessionHistoryDto>> GetVehicleHistoryAsync(string userId, Guid vehicleId)
    {
        var vehicle = await vehicleRepository.FindByIdAsync(vehicleId)
            ?? throw new Exception("Pojazd nie istnieje.");

        if (vehicle.UserId != userId)
            throw new Exception("Brak dostępu do tego pojazdu.");

        var sessions = await sessionRepository.GetParkingSessionHistoryAsync(vehicle.PlateNumber);

        return sessions.Select(s => new ParkingSessionHistoryDto(
            s.Id,
            new VehicleDto(s.Vehicle.Id, s.Vehicle.LicensePlate, s.Vehicle.Brand, s.Vehicle.Color),
            s.GateName,
            s.EntryTime,
            s.ExitTime,
            s.ExitTime.HasValue ? s.ExitTime.Value - s.EntryTime : null,
            s.ParkingFee,
            s.IsActive));
    }

    private static RegisteredVehicleDto ToDto(RegisteredVehicle v) =>
        new(v.Id, v.PlateNumber, v.Brand, v.RegisteredAt);
}
