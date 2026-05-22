using AppCore.Dto;
using AppCore.Models;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class DiscountService(
    IDriverDiscountRepository discountRepository,
    UserManager<AppUser> userManager,
    ParkingDbContext context) : IDiscountService
{
    public async Task<IEnumerable<DriverDiscountDto>> GetDiscountsAsync(string userId)
    {
        var discounts = await discountRepository.GetByUserIdAsync(userId);
        return discounts.Select(ToDto);
    }

    public async Task<DriverDiscountDto> ActivateAsync(string userId, DiscountType type)
    {
        if (type == DiscountType.LoyaltyDiscount)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new Exception("Użytkownik nie istnieje.");

            if (user.TotalSessions < DiscountConstants.LoyaltyDiscount.RequiredSessions)
                throw new Exception($"Wymagane {DiscountConstants.LoyaltyDiscount.RequiredSessions} sesji. Masz {user.TotalSessions}.");
        }

        var existing = await discountRepository.GetActiveByUserIdAndTypeAsync(userId, type);
        if (existing is not null)
            throw new Exception($"Rabat {type} jest już aktywny do {existing.ExpiresAt:d}.");

        var available = (await discountRepository.GetByUserIdAsync(userId))
            .FirstOrDefault(d => d.Type == type && !d.IsActive && d.ActivatedAt is null)
            ?? throw new Exception($"Brak dostępnego rabatu {type} do aktywacji.");

        available.IsActive = true;
        available.ActivatedAt = DateTime.UtcNow;
        available.ExpiresAt = DateTime.UtcNow.AddDays(
            type == DiscountType.RegistrationBonus
                ? DiscountConstants.RegistrationBonus.ValidDays
                : DiscountConstants.LoyaltyDiscount.ValidDays);

        await discountRepository.UpdateAsync(available.Id, available);
        await context.SaveChangesAsync();

        return ToDto(available);
    }

    public async Task GrantRegistrationBonusAsync(string userId)
    {
        var existing = (await discountRepository.GetByUserIdAsync(userId))
            .Any(d => d.Type == DiscountType.RegistrationBonus);

        if (existing) return;

        var discount = new DriverDiscount
        {
            UserId = userId,
            Type = DiscountType.RegistrationBonus,
            IsActive = false
        };

        await discountRepository.AddAsync(discount);
        await context.SaveChangesAsync();
    }

    public async Task<decimal> ApplyDiscountsAsync(string userId, decimal fee, int freeMinutes)
    {
        var bonusMinutes = 0;
        var reductionPercent = 0m;

        var registrationBonus = await discountRepository
            .GetActiveByUserIdAndTypeAsync(userId, DiscountType.RegistrationBonus);
        if (registrationBonus is not null)
            bonusMinutes = DiscountConstants.RegistrationBonus.ExtraFreeMinutes;

        var loyaltyDiscount = await discountRepository
            .GetActiveByUserIdAndTypeAsync(userId, DiscountType.LoyaltyDiscount);
        if (loyaltyDiscount is not null)
            reductionPercent = DiscountConstants.LoyaltyDiscount.PriceReductionPercent;

        // Przelicz opłatę z uwzględnieniem dodatkowych darmowych minut
        var totalFreeMinutes = freeMinutes + bonusMinutes;
        var adjustedFee = RecalculateFee(fee, freeMinutes, totalFreeMinutes);

        // Zastosuj zniżkę procentową
        if (reductionPercent > 0)
            adjustedFee *= (1 - reductionPercent / 100m);

        return Math.Max(0, adjustedFee);
    }

    private static decimal RecalculateFee(decimal originalFee, int originalFreeMinutes, int newFreeMinutes)
    {
        if (originalFee == 0) return 0;
        var extraFreeHours = (newFreeMinutes - originalFreeMinutes) / 60m;
        return Math.Max(0, originalFee - extraFreeHours * TariffConstants.HourlyRate);
    }

    private static DriverDiscountDto ToDto(DriverDiscount d) =>
        new(d.Id, d.Type, d.IsActive, d.ActivatedAt, d.ExpiresAt);
}
