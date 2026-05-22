using AppCore.Models;

namespace AppCore.Repositories;

public interface IRegisteredVehicleRepository : IGenericRepositoryAsync<RegisteredVehicle>
{
    Task<IEnumerable<RegisteredVehicle>> GetByUserIdAsync(string userId);
    Task<RegisteredVehicle?> GetByUserIdAndPlateAsync(string userId, string plateNumber);
}
