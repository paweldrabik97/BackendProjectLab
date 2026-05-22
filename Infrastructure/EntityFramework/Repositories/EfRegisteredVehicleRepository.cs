using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfRegisteredVehicleRepository(ParkingDbContext context)
    : EfGenericRepository<RegisteredVehicle>(context.RegisteredVehicles), IRegisteredVehicleRepository
{
    public async Task<IEnumerable<RegisteredVehicle>> GetByUserIdAsync(string userId) =>
        await context.RegisteredVehicles
            .Where(v => v.UserId == userId)
            .ToListAsync();

    public async Task<RegisteredVehicle?> GetByUserIdAndPlateAsync(string userId, string plateNumber) =>
        await context.RegisteredVehicles
            .FirstOrDefaultAsync(v => v.UserId == userId && v.PlateNumber == plateNumber);
}
