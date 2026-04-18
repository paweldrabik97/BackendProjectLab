using AppCore.Dto;
using Microsoft.AspNetCore.Mvc;
using AppCore.Repositories;

namespace WebApi.Controller;


[ApiController]
[Route("/api/[controller]")]
public class GatesController(IParkingGateService service): ControllerBase
{

    public  async Task<IActionResult> GetAllGates([FromQuery] int page, [FromQuery] int size)
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
        var result = await service.AddGate(dto); // dodaj dto za pomoca metody serwisu
        return CreatedAtAction(nameof(GetGate), new { id = result.Id }, result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateGate(Guid id, UpdateGateDto dto)
    {
        
    }
}