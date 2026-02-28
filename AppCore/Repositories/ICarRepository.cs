using AppCore.Models;
using AppCore.ValueObjects;

namespace AppCore.Repositories;

public interface ICarRepository : IGenericRepository<Car>
{
    Task<Car?> FindByPlateNumber(string plate);
}