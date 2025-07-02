using System.ComponentModel.DataAnnotations;

namespace AuctionApi.Dtos;

public class StartAuctionDto
{
    [Required]
    public Guid VehicleId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal StartingBid { get; set; }
}
