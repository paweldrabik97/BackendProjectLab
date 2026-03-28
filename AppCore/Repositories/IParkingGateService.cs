using AppCore.Models;
using AppCore.Dto;

namespace AppCore.Repositories;

public interface IParkingGateService
{
    Task<List<ParkingGateDto>> GetAll();
    Task<ParkingGateDto?> GetById(Guid id);
    Task<ParkingGateDto?> GetByName(string name);
    Task<ParkingGateDto?> AddGate(CreateGateDto dto);
    Task<ParkingGateDto?> ChangeGateIsOperational(bool isOperational);
}