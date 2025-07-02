using AuctionApi.Dtos;
using AuctionApi.Exceptions;

namespace AuctionApi.Models;

public class Sedan : Vehicle
{
    public int Doors { get; init; }
    public override VehicleTypeDto Type => VehicleTypeDto.Sedan;

    public Sedan(CreateVehicleRequest dto)
        : base(dto.Id, dto.Manufacturer, dto.Model, dto.Year, dto.StartingBid)
    {
        if (dto.Doors is not > 0)
            throw new DomainValidationException("Sedan must have a valid number of doors.");

        Doors = dto.Doors.Value;
    }
}