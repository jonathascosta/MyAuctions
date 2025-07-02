using System.ComponentModel.DataAnnotations;

namespace AuctionApi.Dtos;
public class CreateVehicleRequest
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public VehicleTypeDto Type { get; set; }

    [Required]
    public string Manufacturer { get; set; } = null!;

    [Required]
    public string Model { get; set; } = null!;

    [Range(1886, 2100)]
    public int Year { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal StartingBid { get; set; }

    public int? Doors { get; set; }
    public int? Seats { get; set; }
    public decimal? LoadCapacity { get; set; }
}