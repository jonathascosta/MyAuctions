namespace AuctionApi.Exceptions;

public class AuctionNotActiveException : Exception
{
    public AuctionNotActiveException()
        : base("The auction is not active.")
    {
    }
}
