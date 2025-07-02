using MyAuctions.AuctionHub.Models;
using Newtonsoft.Json;
using System.Text;

namespace MyAuctions.AuctionHub.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;

    public ApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7283/api";
    }

    private void SetAuthHeader(string? token)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
    }

    // Authentication
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        SetAuthHeader(null);
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/login", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AuthResponse>(responseContent);
        }

        return null;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        SetAuthHeader(null);
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/users", content);
        return response.IsSuccessStatusCode;
    }

    // Vehicles
    public async Task<List<Vehicle>> GetVehiclesAsync(string? token = null)
    {
        SetAuthHeader(token);

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/vehicles");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Vehicle>>(content) ?? [];
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting vehicles: {ex.Message}");
        }

        return [];
    }

    public async Task<Vehicle?> GetVehicleAsync(Guid id, string? token = null)
    {
        SetAuthHeader(token);

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/vehicles/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Vehicle>(content);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting vehicle: {ex.Message}");
        }

        return null;
    }

    public async Task<bool> CreateVehicleAsync(CreateVehicleRequest request, string token)
    {
        SetAuthHeader(token);
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/vehicles", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating vehicle: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Vehicle>> SearchVehiclesAsync(VehicleSearchQuery query, string? token = null)
    {
        SetAuthHeader(token);

        try
        {
            var queryString = BuildQueryString(query);
            var response = await _httpClient.GetAsync($"{_baseUrl}/vehicles/{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Vehicle>>(content) ?? [];
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching vehicles: {ex.Message}");
        }

        return [];
    }

    // Auctions
    public async Task<List<Auction>> GetAuctionsAsync(string? token = null)
    {
        SetAuthHeader(token);

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/auctions");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Auction>>(content) ?? [];
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting auctions: {ex.Message}");
        }

        return [];
    }

    public async Task<Auction?> GetAuctionAsync(Guid id, string? token = null)
    {
        SetAuthHeader(token);

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/auctions/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Auction>(content);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting auction: {ex.Message}");
        }

        return null;
    }

    public async Task<bool> StartAuctionAsync(StartAuctionRequest request, string token)
    {
        SetAuthHeader(token);
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/auctions", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting auction: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> StopAuctionAsync(Guid auctionId, string token)
    {
        SetAuthHeader(token);

        try
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/auctions/{auctionId}/stop", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping auction: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> PlaceBidAsync(Guid auctionId, PlaceBidRequest request, string token)
    {
        SetAuthHeader(token);
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/auctions/{auctionId}/bids", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error placing bid: {ex.Message}");
            return false;
        }
    }

    private static string BuildQueryString(VehicleSearchQuery query)
    {
        var queryParams = new List<string>();

        if (query.Type.HasValue)
            queryParams.Add($"type={query.Type}");

        if (!string.IsNullOrEmpty(query.Manufacturer))
            queryParams.Add($"manufacturer={Uri.EscapeDataString(query.Manufacturer)}");

        if (!string.IsNullOrEmpty(query.Model))
            queryParams.Add($"model={Uri.EscapeDataString(query.Model)}");

        if (query.Year.HasValue)
            queryParams.Add($"year={query.Year}");

        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
    }
}

