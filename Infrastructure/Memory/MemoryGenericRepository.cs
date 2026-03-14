using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Models;
using AppCore.Repositories;
using AppCore.Wrappers;

namespace Infrastructure.Memory;

public class MemoryGenericRepository<T> : IGenericRepositoryAsync<T> where T : EntityBase
{
    protected Dictionary<Guid, T> _data = new();

    public Task<T?> FindByIdAsync(Guid id)
    {
        _data.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<T>> FindAllAsync()
    {
        return Task.FromResult<IEnumerable<T>>(_data.Values.ToList());
    }

    public Task<PagedResult<T>> FindPagedAsync(int page, int pageSize)
    {
        var items = _data.Values
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalCount = _data.Count;

        var result = new PagedResult<T>(items, totalCount, page, pageSize);
        return Task.FromResult(result);
    }

    public Task<T> AddAsync(T entity)
    {
        if (_data.ContainsKey(entity.Id))
        {
            throw new InvalidOperationException($"Encja z kluczem {entity.Id} już istnieje.");
        }

        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        _data.Add(entity.Id, entity);
        return Task.FromResult(entity);
    }

    public Task<T> UpdateAsync(Guid id, T entity)
    {
        if (!_data.ContainsKey(id))
        {
            throw new KeyNotFoundException($"Nie można zaktualizować. Brak encji o ID {id}.");
        }

        entity.Id = id; 
        _data[id] = entity;
        
        return Task.FromResult(entity);
    }

    public Task RemoveByIdAsync(Guid id)
    {
        if (!_data.Remove(id))
        {
            throw new KeyNotFoundException($"Nie można usunąć. Brak encji o ID {id}.");
        }

        return Task.CompletedTask;
    }
}