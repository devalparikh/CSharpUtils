using StoreExample.ErrorHandling;
using StoreExample.Models;

namespace StoreExample.Repository;

public interface IDatabase<T> where T : notnull
{

    public static readonly string TableName = nameof(T);
    
    public ErrorOr<IEnumerable<T>> GetAll();
    public ErrorOr<T> GetById(Guid id);
    
    public ErrorOr<T> Upsert(T entry);
}