using AppCore.Models;

namespace AppCore.Repositories;

public interface IVehicleRepository : IGenericRepositoryAsync<Vehicle>
{
    Task<Vehicle?> FindByPlateNumber(string plate);
}