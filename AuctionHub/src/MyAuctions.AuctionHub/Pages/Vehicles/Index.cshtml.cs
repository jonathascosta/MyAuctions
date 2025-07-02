using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAuctions.AuctionHub.Models;
using MyAuctions.AuctionHub.Services;

namespace MyAuctions.AuctionHub.Pages.Vehicles;

public class VehiclesIndexModel(ApiService apiService) : PageModel
{
    private readonly ApiService _apiService = apiService;

    public List<Vehicle> Vehicles { get; set; } = [];
    public VehicleSearchQuery SearchQuery { get; set; } = new();

    public async Task OnGetAsync(VehicleType? type, string? manufacturer, string? model, int? year)
    {
        SearchQuery = new VehicleSearchQuery
        {
            Type = type,
            Manufacturer = manufacturer,
            Model = model,
            Year = year
        };

        var token = User.FindFirst("AccessToken")?.Value;

        try
        {
            if (HasSearchCriteria())
            {
                Vehicles = await _apiService.SearchVehiclesAsync(SearchQuery, token);
            }
            else
            {
                Vehicles = await _apiService.GetVehiclesAsync(token);
            }
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load vehicles. Please check your API configuration.";
            Vehicles = [];
        }
    }

    private bool HasSearchCriteria()
    {
        return SearchQuery.Type.HasValue ||
               !string.IsNullOrEmpty(SearchQuery.Manufacturer) ||
               !string.IsNullOrEmpty(SearchQuery.Model) ||
               SearchQuery.Year.HasValue;
    }
}


