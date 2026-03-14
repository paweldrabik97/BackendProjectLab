using AppCore.Models;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class InMemoryVehicleRepository<T> : MemoryGenericRepository<Vehicle>, IVehicleRepository
{
    public Task<Vehicle?> FindByPlateNumber(string plate)
    {
        var vehicle = _data.Values.FirstOrDefault(v => v.LicensePlate == plate);
        
        return Task.FromResult(vehicle);
    }
}