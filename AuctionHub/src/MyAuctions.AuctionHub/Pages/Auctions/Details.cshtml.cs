using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAuctions.AuctionHub.Models;
using MyAuctions.AuctionHub.Services;

namespace MyAuctions.AuctionHub.Pages.Auctions;

public class AuctionDetailsModel(ApiService apiService) : PageModel
{
    private readonly ApiService _apiService = apiService;

    public Auction? Auction { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var token = User.FindFirst("AccessToken")?.Value;

        try
        {
            Auction = await _apiService.GetAuctionAsync(id, token);

            if (Auction == null)
            {
                TempData["ErrorMessage"] = "Auction not found.";
                return RedirectToPage("/Auctions/Index");
            }

            return Page();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load auction details. Please check your API configuration.";
            return RedirectToPage("/Auctions/Index");
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

        return RedirectToPage(new { id });
    }
}



