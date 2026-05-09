using AppCore.Dto;
using Microsoft.AspNetCore.Mvc;
using AppCore.Repositories;

namespace WebApi.Controller;


[ApiController]
[Route("/api/[controller]")]
public class GatesController(IParkingGateService service): ControllerBase
{

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
}