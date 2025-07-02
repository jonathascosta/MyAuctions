namespace AuctionApi.Exceptions;

public class AuctionNotFoundException : Exception
{
    public AuctionNotFoundException()
        : base("Auction not found.")
    {
    }
}
