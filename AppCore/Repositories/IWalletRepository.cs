using AppCore.Models;

namespace AppCore.Repositories;

public interface IWalletRepository : IGenericRepositoryAsync<WalletTransaction>
{
    Task<IEnumerable<WalletTransaction>> GetByUserIdAsync(string userId);
}
