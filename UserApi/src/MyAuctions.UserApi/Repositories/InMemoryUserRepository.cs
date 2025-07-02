namespace UserApi.Repositories;

using UserApi.Models;
using System.Collections.Concurrent;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _users = new();

    public Task AddAsync(User user)
    {
        if (!_users.TryAdd(user.Username, user))
            throw new Exception("User already exists");
            
        return Task.CompletedTask;
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        _users.TryGetValue(username, out var user);
        return Task.FromResult(user);
    }
}