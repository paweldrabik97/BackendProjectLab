using AppCore.Dto;
using AppCore.Models;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class WalletService(
    IWalletRepository walletRepository,
    UserManager<AppUser> userManager,
    ParkingDbContext context) : IWalletService
{
    public async Task<WalletDto> GetWalletAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new Exception("Użytkownik nie istnieje.");
        return new WalletDto(user.WalletBalance);
    }

    public async Task<WalletDto> TopUpAsync(string userId, decimal amount)
    {
        if (amount <= 0)
            throw new Exception("Kwota doładowania musi być większa od zera.");

        var user = await userManager.FindByIdAsync(userId)
            ?? throw new Exception("Użytkownik nie istnieje.");

        user.WalletBalance += amount;
        await userManager.UpdateAsync(user);

        await walletRepository.AddAsync(new WalletTransaction
        {
            UserId = userId,
            Amount = amount,
            Type = TransactionType.TopUp,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        return new WalletDto(user.WalletBalance);
    }

    public async Task<WalletDto> PayFromWalletAsync(string userId, Guid sessionId, decimal amount)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new Exception("Użytkownik nie istnieje.");

        if (user.WalletBalance < amount)
            throw new Exception($"Niewystarczające saldo. Wymagane: {amount:F2} PLN, dostępne: {user.WalletBalance:F2} PLN.");

        user.WalletBalance -= amount;
        await userManager.UpdateAsync(user);

        await walletRepository.AddAsync(new WalletTransaction
        {
            UserId = userId,
            Amount = amount,
            Type = TransactionType.SessionPayment,
            CreatedAt = DateTime.UtcNow,
            SessionId = sessionId
        });
        await context.SaveChangesAsync();

        return new WalletDto(user.WalletBalance);
    }

    public async Task<IEnumerable<WalletTransactionDto>> GetTransactionsAsync(string userId)
    {
        var transactions = await walletRepository.GetByUserIdAsync(userId);
        return transactions.Select(t => new WalletTransactionDto(
            t.Id, t.Amount, t.Type, t.CreatedAt, t.SessionId));
    }
}
