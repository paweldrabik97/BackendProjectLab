using AppCore.Models;
using AppCore.Wrappers;

namespace AppCore.Repositories;

public interface IGenericRepositoryAsync<T> where T : EntityBase
{
    Task<T?> FindByIdAsync(Guid id);
    Task<IEnumerable<T>> FindAllAsync();
    Task<PagedResult<T>> FindPagedAsync(int page, int pageSize);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(Guid id,T entity);
    Task RemoveByIdAsync(Guid id);
}