namespace AuctionApi.Exceptions;

public class DuplicateVehicleException : Exception
{
    public DuplicateVehicleException(Guid vehicleId)
        : base($"A vehicle with ID {vehicleId} already exists.")
    {
    }
}