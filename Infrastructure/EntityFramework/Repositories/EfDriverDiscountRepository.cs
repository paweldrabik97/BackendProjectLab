using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfDriverDiscountRepository(ParkingDbContext context)
    : EfGenericRepository<DriverDiscount>(context.DriverDiscounts), IDriverDiscountRepository
{
    public async Task<IEnumerable<DriverDiscount>> GetByUserIdAsync(string userId) =>
        await context.DriverDiscounts
            .Where(d => d.UserId == userId)
            .ToListAsync();

    public async Task<DriverDiscount?> GetActiveByUserIdAndTypeAsync(string userId, DiscountType type) =>
        await context.DriverDiscounts
            .FirstOrDefaultAsync(d =>
                d.UserId == userId &&
                d.Type == type &&
                d.IsActive &&
                d.ExpiresAt > DateTime.UtcNow);
}
