using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAuctions.AuctionHub.Models;
using MyAuctions.AuctionHub.Services;

namespace MyAuctions.AuctionHub.Pages.Auctions;

public class AuctionsIndexModel(ApiService apiService) : PageModel
{
    private readonly ApiService _apiService = apiService;

    public List<Auction> Auctions { get; set; } = [];

    public async Task OnGetAsync()
    {
        var token = User.FindFirst("AccessToken")?.Value;

        try
        {
            Auctions = await _apiService.GetAuctionsAsync(token);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load auctions. Please check your API configuration.";
            Auctions = [];
        }
    }

    public async Task<IActionResult> OnPostStopAuctionAsync(Guid id)
    {
        if (!User.IsInRole("Admin"))
        {
            TempData["ErrorMessage"] = "Access denied. Only administrators can stop auctions.";
            return RedirectToPage();
        }

        var token = User.FindFirst("AccessToken")?.Value;
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Please login to continue.";
            return RedirectToPage("/Auth/Login");
        }

        try
        {
            var success = await _apiService.StopAuctionAsync(id, token);

            if (success)
            {
                TempData["SuccessMessage"] = "Auction stopped successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to stop auction. Please try again.";
            }
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to stop auction. Please check your API configuration.";
        }

        return RedirectToPage();
    }
}



