using AppCore.Dto;
using AppCore.Models;

namespace AppCore.Services;

public interface IDiscountService
{
    Task<IEnumerable<DriverDiscountDto>> GetDiscountsAsync(string userId);
    Task<DriverDiscountDto> ActivateAsync(string userId, DiscountType type);
    Task GrantRegistrationBonusAsync(string userId);
    Task<decimal> ApplyDiscountsAsync(string userId, decimal fee, int freeMinutes);
}
