using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAuctions.AuctionHub.Models;
using MyAuctions.AuctionHub.Services;

namespace MyAuctions.AuctionHub.Pages.Auctions;

public class PlaceBidModel(ApiService apiService) : PageModel
{
    private readonly ApiService _apiService = apiService;

    [BindProperty]
    public PlaceBidRequest BidRequest { get; set; } = new();

    public Auction? Auction { get; set; }
    public decimal MinimumBid { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        if (!User.IsInRole("Bidder"))
        {
            TempData["ErrorMessage"] = "Access denied. Only bidders can place bids.";
            return RedirectToPage("/Auctions/Index");
        }

        var token = User.FindFirst("AccessToken")?.Value;

        try
        {
            Auction = await _apiService.GetAuctionAsync(id, token);

            if (Auction == null)
            {
                TempData["ErrorMessage"] = "Auction not found.";
                return RedirectToPage("/Auctions/Index");
            }

            if (Auction.Status != AuctionStatus.Active)
            {
                TempData["ErrorMessage"] = "This auction is no longer active.";
                return Page();
            }

            // Set minimum bid (current bid + 0.01 or starting bid)
            MinimumBid = Auction.CurrentBid.HasValue ? Auction.CurrentBid.Value + 0.01m : Auction.StartingBid;

            return Page();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load auction details. Please check your API configuration.";
            return RedirectToPage("/Auctions/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        if (!User.IsInRole("Bidder"))
        {
            TempData["ErrorMessage"] = "Access denied. Only bidders can place bids.";
            return RedirectToPage("/Auctions/Index");
        }

        var token = User.FindFirst("AccessToken")?.Value;
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Please login to continue.";
            return RedirectToPage("/Auth/Login");
        }

        // Reload auction data for validation
        try
        {
            Auction = await _apiService.GetAuctionAsync(id, token);

            if (Auction == null)
            {
                TempData["ErrorMessage"] = "Auction not found.";
                return RedirectToPage("/Auctions/Index");
            }

            if (Auction.Status != AuctionStatus.Active)
            {
                TempData["ErrorMessage"] = "This auction is no longer active.";
                return Page();
            }

            MinimumBid = Auction.CurrentBid.HasValue ? Auction.CurrentBid.Value + 0.01m : Auction.StartingBid;

            // Validate bid amount
            if (BidRequest.Amount < MinimumBid)
            {
                ModelState.AddModelError("BidRequest.Amount", $"Bid must be at least ${MinimumBid:N2}");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var success = await _apiService.PlaceBidAsync(id, BidRequest, token);

            if (success)
            {
                TempData["SuccessMessage"] = $"Bid of ${BidRequest.Amount:N2} placed successfully!";
                return RedirectToPage("/Auctions/Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to place bid. You may have been outbid or the auction may have ended.";
                return Page();
            }
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to place bid. Please check your API configuration.";
            return Page();
        }
    }
}


