using AuctionApi.Exceptions;
using AuctionApi.Dtos;

namespace AuctionApi.Models;

public class Hatchback : Vehicle
{
    public int Doors { get; }
    public override VehicleTypeDto Type => VehicleTypeDto.Hatchback;

    public Hatchback(CreateVehicleRequest dto)
        : base(dto.Id, dto.Manufacturer, dto.Model, dto.Year, dto.StartingBid)
    {
        if (dto.Doors is not > 0)
            throw new DomainValidationException("Hatchback must have a valid number of doors.");

        Doors = dto.Doors.Value;
    }
}