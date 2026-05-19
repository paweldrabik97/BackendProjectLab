using AppCore.Models;
using AppCore.Dto;
using AppCore.Wrappers;

namespace AppCore.Repositories;

public interface IParkingGateService
{
    Task<PagedResult<ParkingGateDto>> GetAll(int page, int pageSize);
    Task<ParkingGateDto?> GetById(Guid id);
    Task<ParkingGateDto?> GetByName(string name);
    Task<ParkingGateDto?> AddGate(CreateGateDto dto);
    Task<ParkingGateDto?> ChangeGateIsOperational(Guid id, bool isOperational);
    Task<ParkingGateDto?> UpdateGate(Guid id, UpdateGateDto dto);
    Task<CameraCaptureDto> AddCapture(Guid gateId, CreateCameraCaptureDto dto);
    Task<IEnumerable<CameraCaptureDto>> GetCaptures(Guid gateId);
    Task DeleteCapture(Guid gateId, Guid captureId);
}