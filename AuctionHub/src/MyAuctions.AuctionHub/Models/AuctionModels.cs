namespace MyAuctions.AuctionHub.Models;

public enum AuctionStatus
{
    Active,
    Ended
}

public class Auction
{
    public Guid Id { get; set; }
    public Vehicle Vehicle { get; set; } = new();
    public decimal StartingBid { get; set; }
    public decimal? CurrentBid { get; set; }
    public string? CurrentBidder { get; set; }
    public AuctionStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public List<Bid> Bids { get; set; } = [];
}

public class Bid
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Bidder { get; set; } = string.Empty;
}

public class StartAuctionRequest
{
    public Guid VehicleId { get; set; }
    public decimal StartingBid { get; set; }
}

public class PlaceBidRequest
{
    public decimal Amount { get; set; }
}

