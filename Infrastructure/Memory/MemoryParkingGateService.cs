using AppCore.Dto;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class MemoryParkingGateService(IParkingUnitOfWork unit) : IParkingGateService
{
    public Task<List<ParkingGateDto>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<ParkingGateDto?> GetById(Guid id)
    {
        var entity = await unit.Gates.FindByIdAsync(id);
        if (entity is null)
        {
            return null;
        }
        return await Task.FromResult(new ParkingGateDto(
            entity.Id,
            entity.Name,
            entity.Type.ToString(),
            entity.Location,
            entity.IsOperational)
        );
    }

    public async Task<ParkingGateDto?> GetByName(string name)
    {
        var entity = await unit.Gates.FindByParkingGateName(name);
        if (entity is null)
        {
            return null;
        }
        return await Task.FromResult(new ParkingGateDto(
            entity.Id,
            entity.Name,
            entity.Type.ToString(),
            entity.Location,
            entity.IsOperational)
        );
    }

    public Task<ParkingGateDto?> AddGate(CreateGateDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<ParkingGateDto?> ChangeGateIsOperational(bool isOperational)
    {
        throw new NotImplementedException();
    }
}