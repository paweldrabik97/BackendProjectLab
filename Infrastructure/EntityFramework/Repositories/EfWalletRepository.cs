using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfWalletRepository(ParkingDbContext context)
    : EfGenericRepository<WalletTransaction>(context.WalletTransactions), IWalletRepository
{
    public async Task<IEnumerable<WalletTransaction>> GetByUserIdAsync(string userId) =>
        await context.WalletTransactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
}
