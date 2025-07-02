namespace AuctionApi.Models;

public record Bid(string Bidder, decimal Amount, DateTimeOffset Timestamp);