using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAuctions.AuctionHub.Models;
using MyAuctions.AuctionHub.Services;

namespace MyAuctions.AuctionHub.Pages.Auth;

public class LoginModel(ApiService apiService) : PageModel
{
    private readonly ApiService _apiService = apiService;

    [BindProperty]
    public LoginRequest LoginRequest { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var authResponse = await _apiService.LoginAsync(LoginRequest);

            if (authResponse != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, authResponse.Login),
                    new("AccessToken", authResponse.Token),
                    new(ClaimTypes.Role, authResponse.Role.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                TempData["SuccessMessage"] = "Login successful!";
                return RedirectToPage("/Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid username or password.";
                return Page();
            }
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Login failed. Please check your API configuration.";
            return Page();
        }
    }
}
