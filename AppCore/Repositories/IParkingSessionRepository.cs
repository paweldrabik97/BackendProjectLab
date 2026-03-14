using AppCore.Models;

namespace AppCore.Repositories;

public interface IParkingSessionRepository : IGenericRepositoryAsync<ParkingSession>
{
    Task<ParkingSession?> GetByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<ParkingSession>> GetAllActiveAsync();
    Task<IEnumerable<ParkingSession>> GetParkingSessionHistoryAsync(string licensePlate);
}