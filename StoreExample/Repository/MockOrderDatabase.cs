using System.Collections.Concurrent;
using StoreExample.ErrorHandling;
using StoreExample.Models;

namespace StoreExample.Repository;

public abstract class MockOrderDatabase : IDatabase<OrderEntity> 
{
    public static readonly string TableName = "Orders";

    private static readonly ConcurrentDictionary<Guid, OrderEntity> DbContext = new();
    
    public ErrorOr<IEnumerable<OrderEntity>> GetAll()
    {
        return DbContext.Values.ToArray();
    }

    public ErrorOr<OrderEntity> GetById(Guid id)
    {
        if (!DbContext.TryGetValue(id, out OrderEntity? orderEntity))
        {
            return new Error(ErrorType.NotFound, [$"{nameof(OrderEntity)} with id {id} is not found"]);
        }
        return orderEntity;
    }
    
    public ErrorOr<OrderEntity> Create(OrderEntity entry)
    {
        if (DbContext.TryGetValue(entry.Id, out OrderEntity? orderEntity))
        {
            return new Error(ErrorType.Conflict, [$"{nameof(OrderEntity)} with id {orderEntity.Id} already exists"]);
        }

        DbContext[entry.Id] = entry;

        return DbContext[entry.Id];
    }

    public ErrorOr<OrderEntity> Upsert(OrderEntity entry)
    {
        throw new NotImplementedException();
    }

    public ErrorOr<OrderEntity> Update(OrderEntity entry)
    {
        if (!DbContext.TryGetValue(entry.Id, out OrderEntity? orderEntity))
        {
            return new Error(ErrorType.NotFound, [$"{nameof(OrderEntity)} with id {entry.Id} is not found"]);
        }

        DbContext[entry.Id] = entry;

        return DbContext[entry.Id];
    }
}