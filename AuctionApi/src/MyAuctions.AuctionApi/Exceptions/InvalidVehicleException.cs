namespace AuctionApi.Exceptions;

public class InvalidVehicleException : Exception
{
    public InvalidVehicleException(Guid vehicleId)
        : base($"Vehicle with ID {vehicleId} is invalid or does not exist.")
    {
    }
}