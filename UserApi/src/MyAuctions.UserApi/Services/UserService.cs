using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using UserApi.Dtos;
using UserApi.Models;
using UserApi.Repositories;

namespace UserApi.Services;

public class UserService(IUserRepository userRepository, PasswordHasher<User> passwordHasher, IConfiguration config) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly PasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly IConfiguration _config = config;

    public async Task RegisterAsync(CreateUserRequest dto)
    {
        var existing = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existing is not null)
            throw new ApplicationException("Username already taken.");

        var user = new User { Username = dto.Username, Role = dto.Role };
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        await _userRepository.AddAsync(user);
    }

    public async Task<string> LoginAsync(LoginRequest dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username)
            ?? throw new ApplicationException("Invalid credentials.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new ApplicationException("Invalid credentials.");

        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);
        var expiryMinutes = _config.GetValue<int>("Jwt:ExpiresMinutes");
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}