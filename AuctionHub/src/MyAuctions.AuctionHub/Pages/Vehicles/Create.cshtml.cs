using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAuctions.AuctionHub.Models;
using MyAuctions.AuctionHub.Services;

namespace MyAuctions.AuctionHub.Pages.Vehicles;

public class CreateVehicleModel(ApiService apiService) : PageModel
{
    private readonly ApiService _apiService = apiService;

    [BindProperty]
    public CreateVehicleRequest CreateRequest { get; set; } = new();

    public IActionResult OnGet()
    {
        // Check if user is admin
        if (!User.IsInRole("Admin"))
        {
            TempData["ErrorMessage"] = "Access denied. Only administrators can add vehicles.";
            return RedirectToPage("/Vehicles/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Check if user is admin
        if (!User.IsInRole("Admin"))
        {
            TempData["ErrorMessage"] = "Access denied. Only administrators can add vehicles.";
            return RedirectToPage("/Vehicles/Index");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var token = User.FindFirst("AccessToken")?.Value;
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Please login to continue.";
            return RedirectToPage("/Auth/Login");
        }

        try
        {
            var success = await _apiService.CreateVehicleAsync(CreateRequest, token);

            if (success)
            {
                TempData["SuccessMessage"] = "Vehicle added successfully!";
                return RedirectToPage("/Vehicles/Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add vehicle. Please try again.";
                return Page();
            }
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to add vehicle. Please check your API configuration.";
            return Page();
        }
    }
}