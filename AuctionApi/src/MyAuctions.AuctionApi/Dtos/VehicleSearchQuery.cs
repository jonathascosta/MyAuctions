namespace AuctionApi.Dtos;

public class VehicleSearchQuery
{
    public VehicleTypeDto? Type { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
}
