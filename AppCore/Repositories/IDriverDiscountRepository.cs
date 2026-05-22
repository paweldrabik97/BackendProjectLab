using AppCore.Models;

namespace AppCore.Repositories;

public interface IDriverDiscountRepository : IGenericRepositoryAsync<DriverDiscount>
{
    Task<IEnumerable<DriverDiscount>> GetByUserIdAsync(string userId);
    Task<DriverDiscount?> GetActiveByUserIdAndTypeAsync(string userId, DiscountType type);
}
