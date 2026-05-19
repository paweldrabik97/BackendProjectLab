using AppCore.Dto;
using AppCore.Exceptions;
using AppCore.Models;
using AppCore.Repositories;
using AppCore.Wrappers;

namespace AppCore.Services;

public class ParkingGateService(IParkingUnitOfWork unit) : IParkingGateService
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
    
    public async Task<ParkingGateDto?> UpdateGate(Guid id, UpdateGateDto dto)
    {
        var entity = await unit.Gates.FindByIdAsync(id);
        if (entity is null)
            return null;

        dto.UpdateExistingEntity(entity);

        var updated = await unit.Gates.UpdateAsync(id, entity);
        await unit.SaveChangesAsync();
    
        return ToDto(updated);
    }
    
    public async Task<CameraCaptureDto> AddCapture(Guid gateId, CreateCameraCaptureDto dto)
    {
        var gate = await unit.Gates.FindByIdAsync(gateId);
    
        if (gate is null)
            throw new GateNotFoundException($"Bramka o id {gateId} nie została znaleziona!");

        var capture = new CameraCapture
        {
            Id = dto.Id,
            LicensePlate = dto.LicensePlate,
            Brand = dto.Brand,
            Color = dto.Color,
            ImagePath = dto.ImagePath
        };

        gate.CameraCaptures.Add(capture);
    
        await unit.Gates.UpdateAsync(gateId, gate);
        await unit.SaveChangesAsync();

        return new CameraCaptureDto(capture.Id, capture.LicensePlate, capture.Brand, capture.Color, gate.Name, capture.ImagePath);
    }

    public async Task<IEnumerable<CameraCaptureDto>> GetCaptures(Guid gateId)
    {
        var gate = await unit.Gates.FindByIdAsync(gateId);
        if (gate is null)
            throw new GateNotFoundException($"Bramka o id {gateId} nie została znaleziona!");

        return gate.CameraCaptures.Select(c => 
            new CameraCaptureDto(c.Id, c.LicensePlate, c.Brand, c.Color, gate.Name, c.ImagePath)).ToList();
    }
    
    public async Task DeleteCapture(Guid gateId, Guid captureId)
    {
        var gate = await unit.Gates.FindByIdAsync(gateId);
        if (gate is null)
            throw new GateNotFoundException($"Bramka o id {gateId} nie została znaleziona!");

        var capture = gate.CameraCaptures.FirstOrDefault(c => c.Id == captureId);
        if (capture is not null)
        {
            gate.CameraCaptures.Remove(capture);
            await unit.Gates.UpdateAsync(gateId, gate);
            await unit.SaveChangesAsync();
        }
    }
}