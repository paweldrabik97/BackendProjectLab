using AppCore.Dto;
using AppCore.Models;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controller;

[ApiController]
[Route("api/driver")]
[Authorize]
public class DriverController(
    IDriverService driverService,
    IDiscountService discountService,
    IWalletService walletService,
    IParkingSessionService sessionService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // ── Pojazdy ───────────────────────────────────────────────

    [HttpPost("vehicles")]
    [ProducesResponseType(typeof(RegisteredVehicleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterVehicle([FromBody] RegisterVehicleDto dto)
    {
        var vehicle = await driverService.RegisterVehicleAsync(UserId, dto);
        return CreatedAtAction(nameof(GetVehicles), vehicle);
    }

    [HttpGet("vehicles")]
    [ProducesResponseType(typeof(IEnumerable<RegisteredVehicleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVehicles()
    {
        var vehicles = await driverService.GetVehiclesAsync(UserId);
        return Ok(vehicles);
    }

    [HttpGet("vehicles/{id:guid}/history")]
    [ProducesResponseType(typeof(IEnumerable<ParkingSessionHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVehicleHistory([FromRoute] Guid id)
    {
        var history = await driverService.GetVehicleHistoryAsync(UserId, id);
        return Ok(history);
    }

    // ── Sesja ─────────────────────────────────────────────────

    [HttpGet("sessions/{plate}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentSession([FromRoute] string plate)
    {
        var session = await sessionService.GetActiveSessionAsync(plate);
        if (session is null)
            return NotFound(new { Message = $"Brak aktywnej sesji dla pojazdu {plate}." });

        return Ok(session);
    }

    [HttpPost("sessions/pay")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PayFromWallet([FromBody] SessionPayDto dto)
    {
        // Zamknij sesję i oblicz opłatę z rabatami
        var result = await sessionService.PayAsync(dto.PlateNumber, UserId);

        // Potrąć z portfela jeśli jest opłata
        WalletDto? wallet = null;
        if (result.Fee > 0)
            wallet = await walletService.PayFromWalletAsync(UserId, result.SessionId, result.Fee);

        return Ok(new { session = result, wallet });
    }

    // ── Rabaty ────────────────────────────────────────────────

    [HttpGet("discounts")]
    [ProducesResponseType(typeof(IEnumerable<DriverDiscountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDiscounts()
    {
        var discounts = await discountService.GetDiscountsAsync(UserId);
        return Ok(discounts);
    }

    [HttpPost("discounts/{type}/activate")]
    [ProducesResponseType(typeof(DriverDiscountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActivateDiscount([FromRoute] DiscountType type)
    {
        var discount = await discountService.ActivateAsync(UserId, type);
        return Ok(discount);
    }

    // ── Portfel ───────────────────────────────────────────────

    [HttpGet("wallet")]
    [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWallet()
    {
        var wallet = await walletService.GetWalletAsync(UserId);
        return Ok(wallet);
    }

    [HttpPost("wallet/topup")]
    [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TopUp([FromBody] TopUpDto dto)
    {
        var wallet = await walletService.TopUpAsync(UserId, dto.Amount);
        return Ok(wallet);
    }

    [HttpGet("wallet/transactions")]
    [ProducesResponseType(typeof(IEnumerable<WalletTransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions()
    {
        var transactions = await walletService.GetTransactionsAsync(UserId);
        return Ok(transactions);
    }
}
