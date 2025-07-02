using System.ComponentModel.DataAnnotations;

namespace AuctionApi.Dtos;

public class PlaceBidDto
{
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}