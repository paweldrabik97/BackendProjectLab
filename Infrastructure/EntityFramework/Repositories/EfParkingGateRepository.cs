using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfParkingGateRepository(ParkingDbContext context) 
    : EfGenericRepository<ParkingGate>(context.Gates), IParkingGateRepository
{
    public async Task<ParkingGate?> FindByParkingGateName(string name)
    {
        return await context.Gates
            .Include(g => g.CameraCaptures) 
            .FirstOrDefaultAsync(g => g.Name == name);
    }

    public new async Task<ParkingGate?> FindByIdAsync(Guid id)
    {
        return await context.Gates
            .Include(g => g.CameraCaptures)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}