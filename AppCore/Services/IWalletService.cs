using AppCore.Dto;

namespace AppCore.Services;

public interface IWalletService
{
    Task<WalletDto> GetWalletAsync(string userId);
    Task<WalletDto> TopUpAsync(string userId, decimal amount);
    Task<WalletDto> PayFromWalletAsync(string userId, Guid sessionId, decimal amount);
    Task<IEnumerable<WalletTransactionDto>> GetTransactionsAsync(string userId);
}
