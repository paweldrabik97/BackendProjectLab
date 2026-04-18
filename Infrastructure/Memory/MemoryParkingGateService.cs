using AppCore.Dto;
using AppCore.Repositories;
using AppCore.Wrappers;
using AppCore.Models;

namespace Infrastructure.Memory;

public class MemoryParkingGateService(IParkingUnitOfWork unit) : IParkingGateService
{
    public async Task<PagedResult<ParkingGateDto>> GetAll(int page, int pageSize)
    {
        var paged = await unit.Gates.FindPagedAsync(page, pageSize);
        var items = paged.Items.Select(ToDto).ToList();
        return new PagedResult<ParkingGateDto>(items, paged.TotalCount, paged.Page, paged.PageSize);
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

    public async Task<ParkingGateDto?> AddGate(CreateGateDto dto)
    {
        var entity = dto.ToEntity();
        var added = await unit.Gates.AddAsync(entity);
        await unit.SaveChangesAsync();
        return ToDto(added);
    }

    public async Task<ParkingGateDto?> ChangeGateIsOperational(Guid id, bool isOperational)
    {
        var entity = await unit.Gates.FindByIdAsync(id);
        if (entity is null)
            throw new KeyNotFoundException($"Nie znaleziono bramki o id {id}");

        entity.IsOperational = isOperational;
        var updated = await unit.Gates.UpdateAsync(id, entity);
        await unit.SaveChangesAsync();
        return ToDto(updated);
    }
    
    private static ParkingGateDto ToDto(ParkingGate entity) =>
        new(
            entity.Id,
            entity.Name,
            entity.Type.ToString(),
            entity.Location,
            entity.IsOperational
        );
}