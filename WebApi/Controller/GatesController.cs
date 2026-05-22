using AppCore.Dto;
using AppCore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controller;


[ApiController]
[Route("/api/[controller]")]
public class GatesController(IParkingGateService service): ControllerBase
{
    [HttpGet("")]
    [Authorize(Policy = nameof(AppPolicies.AdminOnly))]
    public  async Task<IActionResult> GetAllGates([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        return Ok(await service.GetAll(page, size));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGate(Guid id)
    {
        var dto = await service.GetById(id);

        if (dto is null)
            return NotFound();
        
        return Ok(dto);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateGate(CreateGateDto dto)
    {
        var result = await service.AddGate(dto);
        return CreatedAtAction(nameof(GetGate), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGate(Guid id, [FromBody] UpdateGateDto dto)
    {
        var existingGate = await service.GetById(id);
    
        if (existingGate is null)
        {
            return NotFound(new { Message = $"Nie znaleziono bramki o id {id}" });
        }

        var updatedGate = await service.UpdateGate(id, dto);

        return Ok(updatedGate);
    }
    
    [HttpPost("{gateId:guid}/captures")]
    [ProducesResponseType(typeof(CameraCaptureDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCameraCapture([FromRoute] Guid gateId, [FromBody] CreateCameraCaptureDto dto)
    {
        var capture = await service.AddCapture(gateId, dto);
        return CreatedAtAction(nameof(GetCaptures), new { gateId }, capture);
    }

    [HttpGet("{gateId:guid}/captures")]
    [ProducesResponseType(typeof(IEnumerable<CameraCaptureDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCaptures([FromRoute] Guid gateId)
    {
        var captures = await service.GetCaptures(gateId);
        return Ok(captures);
    }
    
    [HttpDelete("{gateId:guid}/captures/{captureId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCapture([FromRoute] Guid gateId, [FromRoute] Guid captureId)
    {
        await service.DeleteCapture(gateId, captureId);
        return NoContent();
    }
}