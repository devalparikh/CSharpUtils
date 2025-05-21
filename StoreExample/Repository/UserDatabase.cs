using System.Collections.Concurrent;
using StoreExample.ErrorHandling;
using StoreExample.Models;

namespace StoreExample.Repository;

public abstract class UserDatabase : IDatabase<UserEntity> 
{
    public static readonly string TableName = "Users";
    
    private static readonly ConcurrentDictionary<Guid, UserEntity> DbContext = new();
    
    public ErrorOr<IEnumerable<UserEntity>> GetAll()
    {
        return DbContext.Values.ToArray();
    }

    public ErrorOr<UserEntity> GetById(Guid id)
    {
        if (!DbContext.TryGetValue(id, out UserEntity? userEntity))
        {
            return new Error(ErrorType.NotFound, [$"{nameof(UserEntity)} with id {id} is not found"]);
        }
        return userEntity;
    }
    
    public ErrorOr<UserEntity> GetByUsername(string username)
    {
        var userEntity = DbContext.Values.FirstOrDefault(u => u.Username == username);

        if (userEntity is null)
        {
            return new Error(ErrorType.NotFound, [$"{nameof(UserEntity)} with username '{username}' is not found"]);
        }
        return userEntity;
    }

    public ErrorOr<UserEntity> Upsert(UserEntity entry)
    {
        DbContext[entry.Id] = entry;
        return DbContext[entry.Id];
    }
}