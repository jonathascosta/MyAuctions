using AuctionApi.Dtos;
using AuctionApi.Exceptions;

namespace AuctionApi.Models;

public class SUV : Vehicle
{
    public int Seats { get; init; }
    public override VehicleTypeDto Type => VehicleTypeDto.SUV;

    public SUV(CreateVehicleRequest dto)
        : base(dto.Id, dto.Manufacturer, dto.Model, dto.Year, dto.StartingBid)
    {
        if (dto.Seats is not > 0)
            throw new DomainValidationException("SUV must have a valid number of seats.");

        Seats = dto.Seats.Value;
    }
}