using AppCore.Dto;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controller;

[ApiController]
[Route("api/sessions")]
public class SessionsController(IParkingSessionService sessionService) : ControllerBase
{
    [HttpPost("entry")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ParkingEntryResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Entry([FromBody] SessionEntryDto dto)
    {
        var result = await sessionService.EntryAsync(dto.PlateNumber, dto.GateName);
        return CreatedAtAction(nameof(GetStatus), new { plate = dto.PlateNumber }, result);
    }

    [HttpGet("{plate}/status")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatus([FromRoute] string plate)
    {
        var session = await sessionService.GetActiveSessionAsync(plate);
        if (session is null)
            return NotFound(new { Message = $"Brak aktywnej sesji dla pojazdu {plate}." });

        return Ok(session);
    }

    [HttpGet("{plate}/history")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ParkingSessionHistoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory([FromRoute] string plate)
    {
        var history = await sessionService.GetSessionHistoryAsync(plate);
        return Ok(history);
    }

    [HttpPost("{plate}/pay")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pay([FromRoute] string plate)
    {
        var result = await sessionService.PayAsync(plate);
        return Ok(result);
    }
}
