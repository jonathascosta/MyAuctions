namespace UserApi.Repositories;

using UserApi.Models;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByUsernameAsync(string username);
}