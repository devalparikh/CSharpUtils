using System.Collections.Concurrent;
using Riok.Mapperly.Abstractions;

namespace StoreExample.Models;


public class User
{
    public Guid Id;
    public string Username;
}

public class UserRequest
{
    public Guid Id;
    public string Username;
}

public class UserResponse
{
    public Guid Id;
    public string Username;
}

public class UserEntity : Entity
{
    public string Username;
}


[Mapper]
public partial class UserMapper
{
    public partial UserEntity ToEntity(User user);
    public partial User ToModel(UserEntity entity);
    public partial User ToModel(UserRequest request);
    public partial UserResponse ToResponse(User user);

    public async Task<IEnumerable<UserResponse>> ToResponses(IEnumerable<User> users)
    {
        var usersResponse = new ConcurrentQueue<UserResponse>(); // Thread-safe and maintains user

        await Parallel.ForEachAsync(users, async (user, _) =>
        {
            var response = ToResponse(user);
            usersResponse.Enqueue(response); // Preserves user
        });

        return usersResponse;
    }
}