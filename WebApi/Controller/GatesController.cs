using Microsoft.AspNetCore.Mvc;
using AppCore.Repositories;

namespace WebApi.Controller;


[ApiController]
[Route("/api/[controller]")]
public class GatesController(IParkingGateService service): ControllerBase
{

    public  async Task<IActionResult> GetAllGates([FromQuery] int page = 1, [FromQuery] int size = 1)
    {
        return Ok(await service.GetAll(page, size));
    }
}