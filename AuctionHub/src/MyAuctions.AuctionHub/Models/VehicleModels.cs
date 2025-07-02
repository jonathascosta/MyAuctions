namespace MyAuctions.AuctionHub.Models;

public enum VehicleType
{
    Sedan,
    Hatchback,
    SUV,
    Truck
}

public class Vehicle
{
    public Guid Id { get; set; }
    public VehicleType Type { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal StartingBid { get; set; }
    public int? Doors { get; set; }
    public int? Seats { get; set; }
    public decimal? LoadCapacity { get; set; }
}

public class CreateVehicleRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public VehicleType Type { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal StartingBid { get; set; }
    public int? Doors { get; set; }
    public int? Seats { get; set; }
    public decimal? LoadCapacity { get; set; }
}

public class VehicleSearchQuery
{
    public VehicleType? Type { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
}

