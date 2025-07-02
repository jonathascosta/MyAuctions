namespace UserApi.Services;

using UserApi.Dtos;

public interface IUserService
{
    Task RegisterAsync(CreateUserRequest dto);
    Task<string> LoginAsync(LoginRequest dto);
}