using AppCore.Models;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class InMemoryParkingGateRepository<T> : MemoryGenericRepository<ParkingGate>, IParkingGateRepository
{
    public Task<ParkingGate?> FindByParkingGateName(string parkingGateName)
    {
        var parkingGate = _data.Values.FirstOrDefault(pg => pg.Name == parkingGateName);
        return Task.FromResult(parkingGate);

    }
    
}