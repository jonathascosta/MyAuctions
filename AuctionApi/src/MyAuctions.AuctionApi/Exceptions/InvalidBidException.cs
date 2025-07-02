namespace AuctionApi.Exceptions;

public class InvalidBidException : Exception
{
    public InvalidBidException(decimal currentPrice)
        : base($"Bid must be greater than the current price: ({currentPrice:C}).")
    {
    }
}
