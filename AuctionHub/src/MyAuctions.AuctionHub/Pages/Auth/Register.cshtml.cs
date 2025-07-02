using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAuctions.AuctionHub.Models;
using MyAuctions.AuctionHub.Services;

namespace MyAuctions.AuctionHub.Pages.Auth;

public class RegisterModel(ApiService apiService) : PageModel
{
    private readonly ApiService _apiService = apiService;

    [BindProperty]
    public RegisterRequest RegisterRequest { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var success = await _apiService.RegisterAsync(RegisterRequest);

            if (success)
            {
                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToPage("/Auth/Login");
            }
            else
            {
                TempData["ErrorMessage"] = "Registration failed. Username may already exist.";
                return Page();
            }
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Registration failed. Please check your API configuration.";
            return Page();
        }
    }
}

