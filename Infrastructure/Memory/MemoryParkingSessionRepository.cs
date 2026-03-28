using AppCore.Models;
using AppCore.Repositories;
using AppCore.Dto;

namespace Infrastructure.Memory;

public class MemoryParkingSessionRepository<T> : MemoryGenericRepository<ParkingSession>, IParkingSessionRepository
{
    public Task<ParkingSession?> GetByLicensePlateAsync(string licensePlate)
    {
        var parkingSession = _data.Values.FirstOrDefault(ps => ps.Vehicle.LicensePlate == licensePlate);
        return Task.FromResult(parkingSession);
    }

    public Task<IEnumerable<ParkingSession>> GetAllActiveAsync()
    {
        IEnumerable<ParkingSession> parkingSessions = _data.Values.Where(ps => ps.IsActive);
        return Task.FromResult(parkingSessions);
    }

    public Task<IEnumerable<ParkingSession>> GetParkingSessionHistoryAsync(string licensePlate)
    {
        IEnumerable<ParkingSession> parkingSession =
            _data.Values.Where(session => session.Vehicle.LicensePlate == licensePlate);
        return Task.FromResult(parkingSession);
    }
}