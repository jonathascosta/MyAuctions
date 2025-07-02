using AuctionApi.Dtos;
using AuctionApi.Exceptions;

namespace AuctionApi.Models;

public class Truck : Vehicle
{
    public decimal LoadCapacity { get; init; }
    public override VehicleTypeDto Type => VehicleTypeDto.Truck;
    
    public Truck(CreateVehicleRequest dto)
        : base(dto.Id, dto.Manufacturer, dto.Model, dto.Year, dto.StartingBid)
    {
        if (dto.LoadCapacity is not > 0)
            throw new DomainValidationException("Truck must have a valid load capacity.");

        LoadCapacity = dto.LoadCapacity.Value;
    }
}