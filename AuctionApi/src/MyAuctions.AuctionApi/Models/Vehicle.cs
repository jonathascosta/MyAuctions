using AuctionApi.Dtos;

namespace AuctionApi.Models;

public abstract class Vehicle
{
    public Guid Id { get; init; }
    public string Manufacturer { get; init; } = null!;
    public string Model { get; init; } = null!;
    public int Year { get; init; }
    public decimal StartingBid { get; init; }
    public abstract VehicleTypeDto Type { get; }

    protected Vehicle(Guid id, string manufacturer, string model, int year, decimal startingBid)
    {
        Id = id;
        Manufacturer = manufacturer;
        Model = model;
        Year = year;
        StartingBid = startingBid;
    }
    
    public static Vehicle Create(CreateVehicleRequest dto) =>
        dto.Type switch
        {
            VehicleTypeDto.Hatchback => new Hatchback(dto),
            VehicleTypeDto.Sedan     => new Sedan(dto),
            VehicleTypeDto.SUV       => new SUV(dto),
            VehicleTypeDto.Truck     => new Truck(dto),
            _ => throw new ApplicationException($"Unsupported vehicle type: {dto.Type}")
        };
}