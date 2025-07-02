namespace AuctionApi.Exceptions;

public class AuctionAlreadyActiveException : Exception
{
    public AuctionAlreadyActiveException()
        : base("Auction already exists.")
    {

    }
}
