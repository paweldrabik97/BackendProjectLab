using AppCore.Dto;

namespace AppCore.Services;

public interface IParkingSessionService
{
    Task<ParkingEntryResultDto> EntryAsync(string plateNumber, string gateName);
    Task<ParkingExitResultDto> ExitAsync(string plateNumber, string gateName);
    Task<ActiveParkingSessionDto?> GetActiveSessionAsync(string plateNumber);
    Task<ParkingExitResultDto> PayAsync(string plateNumber, string? userId = null);
    Task<IEnumerable<ParkingSessionHistoryDto>> GetSessionHistoryAsync(string plateNumber);
}
