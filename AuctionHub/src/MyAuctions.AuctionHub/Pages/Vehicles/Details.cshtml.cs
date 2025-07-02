using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAuctions.AuctionHub.Models;
using MyAuctions.AuctionHub.Services;

namespace MyAuctions.AuctionHub.Pages.Vehicles;

public class VehicleDetailsModel(ApiService apiService) : PageModel
{
    private readonly ApiService _apiService = apiService;

    public Vehicle? Vehicle { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var token = User.FindFirst("AccessToken")?.Value;

        try
        {
            Vehicle = await _apiService.GetVehicleAsync(id, token);

            if (Vehicle == null)
            {
                TempData["ErrorMessage"] = "Vehicle not found.";
                return RedirectToPage("/Vehicles/Index");
            }

            return Page();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load vehicle details. Please check your API configuration.";
            return RedirectToPage("/Vehicles/Index");
        }
    }
}



