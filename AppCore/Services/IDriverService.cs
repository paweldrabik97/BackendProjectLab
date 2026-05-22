using AppCore.Dto;

namespace AppCore.Services;

public interface IDriverService
{
    Task<RegisteredVehicleDto> RegisterVehicleAsync(string userId, RegisterVehicleDto dto);
    Task<IEnumerable<RegisteredVehicleDto>> GetVehiclesAsync(string userId);
    Task<IEnumerable<ParkingSessionHistoryDto>> GetVehicleHistoryAsync(string userId, Guid vehicleId);
}
