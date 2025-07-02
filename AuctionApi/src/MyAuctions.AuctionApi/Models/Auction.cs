using AuctionApi.Exceptions;

namespace AuctionApi.Models;

public class Auction
{
    public Guid Id { get; }
    public Guid VehicleId { get; }
    public decimal StartingBid { get; }
    public DateTimeOffset StartedAt { get; }
    public DateTimeOffset? EndedAt { get; private set; }

    private readonly List<Bid> _bids = new();
    public IReadOnlyList<Bid> Bids => _bids;
    public decimal CurrentPrice => _bids.Any() 
        ? _bids.Max(b => b.Amount) 
        : StartingBid;

    private Auction(Guid vehicleId, decimal startingBid)
    {
        Id = Guid.NewGuid();
        VehicleId = vehicleId;
        StartingBid = startingBid;
        StartedAt = DateTimeOffset.UtcNow;
    }

    public static Auction Create(Guid vehicleId, decimal startingBid) =>
        new Auction(vehicleId, startingBid);

    public void PlaceBid(string bidder, decimal amount)
    {
        if (EndedAt != null)
            throw new AuctionNotActiveException();

        if (amount <= CurrentPrice)
            throw new InvalidBidException(CurrentPrice);

        _bids.Add(new Bid(bidder, amount, DateTimeOffset.UtcNow));
    }

    public void Close()
    {
        if (EndedAt != null)
            throw new AuctionNotActiveException();

        EndedAt = DateTimeOffset.UtcNow;
    }
}
