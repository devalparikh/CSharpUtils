using System.Collections.Concurrent;
using StoreExample.ErrorHandling;
using StoreExample.Models;

namespace StoreExample.Repository;

public abstract class MockEntityDatabase<T> : IDatabase<T> 
    where T : Entity
{
    private static readonly ConcurrentDictionary<Guid, T> DbContext = new();
    
    public ErrorOr<IEnumerable<T>> GetAll()
    {
        return DbContext.Values.ToArray();
    }

    public ErrorOr<T> GetById(Guid id)
    {
        if (!DbContext.TryGetValue(id, out T? orderEntity))
        {
            return new Error(ErrorType.NotFound, [$"{nameof(T)} with id {id} is not found"]);
        }
        return orderEntity;
    }

    public ErrorOr<T> Upsert(T entry)
    {
        DbContext[entry.Id] = entry;
        return DbContext[entry.Id];
    }
}