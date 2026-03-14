using AppCore.Models;

namespace AppCore.Repositories;

public interface IParkingGateRepository : IGenericRepositoryAsync<ParkingGate>
{
    Task<ParkingGate?> FindByParkingGateName(string parkingGateName);
}