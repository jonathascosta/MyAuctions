namespace MyAuctions.AuctionHub.Models;

public enum UserRole
{
    Admin,
    Bidder
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class User
{
    public string Login { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Token { get; set; } = string.Empty;
}

