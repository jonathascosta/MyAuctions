using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAuctions.AuctionHub.Models;
using MyAuctions.AuctionHub.Services;

namespace MyAuctions.AuctionHub.Pages.Auctions;

public class StartAuctionModel(ApiService apiService) : PageModel
{
    private readonly ApiService _apiService = apiService;

    [BindProperty]
    public StartAuctionRequest StartRequest { get; set; } = new();

    public List<Vehicle> AvailableVehicles { get; set; } = [];
    public Vehicle? SelectedVehicle { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? vehicleId)
    {
        if (!User.IsInRole("Admin"))
        {
            TempData["ErrorMessage"] = "Access denied. Only administrators can start auctions.";
            return RedirectToPage("/Auctions/Index");
        }

        var token = User.FindFirst("AccessToken")?.Value;

        try
        {
            if (vehicleId.HasValue)
            {
                SelectedVehicle = await _apiService.GetVehicleAsync(vehicleId.Value, token);
                if (SelectedVehicle != null)
                {
                    StartRequest.VehicleId = vehicleId.Value;
                    StartRequest.StartingBid = SelectedVehicle.StartingBid;
                }
            }
            else
            {
                AvailableVehicles = await _apiService.GetVehiclesAsync(token);
            }
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load vehicle information. Please check your API configuration.";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!User.IsInRole("Admin"))
        {
            TempData["ErrorMessage"] = "Access denied. Only administrators can start auctions.";
            return RedirectToPage("/Auctions/Index");
        }

        if (!ModelState.IsValid)
        {
            await LoadDataAsync();
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
            var success = await _apiService.StartAuctionAsync(StartRequest, token);

            if (success)
            {
                TempData["SuccessMessage"] = "Auction started successfully!";
                return RedirectToPage("/Auctions/Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to start auction. Please try again.";
                await LoadDataAsync();
                return Page();
            }
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to start auction. Please check your API configuration.";
            await LoadDataAsync();
            return Page();
        }
    }

    private async Task LoadDataAsync()
    {
        var token = User.FindFirst("AccessToken")?.Value;

        try
        {
            if (StartRequest.VehicleId != Guid.Empty)
            {
                SelectedVehicle = await _apiService.GetVehicleAsync(StartRequest.VehicleId, token);
            }
            else
            {
                AvailableVehicles = await _apiService.GetVehiclesAsync(token);
            }
        }
        catch (Exception)
        {
            // Error already handled in calling method
        }
    }
}


