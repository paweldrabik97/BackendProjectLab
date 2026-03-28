using AppCore.Enums;
using AppCore.Models;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class MemoryParkingGateRepository<T> : MemoryGenericRepository<ParkingGate>, IParkingGateRepository
{
    public MemoryParkingGateRepository()
    {
        var gate = new ParkingGate()
        {
            Id = Guid.NewGuid(),
            Name = "Entry Gate",
            Type = GateType.Entry,
            Location = "Main Gate",
            IsOperational = false
        };
        _data.Add(gate.Id, gate);

        var secondGate = new ParkingGate()
        {
            Id = Guid.NewGuid(),
            Name = "Exit Gate",
            Type = GateType.Exit,
            Location = "Main Gate",
            IsOperational = false
        };
    }
    public Task<ParkingGate?> FindByParkingGateName(string parkingGateName)
    {
        var parkingGate = _data.Values.FirstOrDefault(pg => pg.Name == parkingGateName);
        return Task.FromResult(parkingGate);

    }
    
}