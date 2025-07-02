using AuctionApi.Dtos;
using AuctionApi.Exceptions;
using AuctionApi.Models;
using AuctionApi.Repositories;

namespace AuctionApi.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    public VehicleService(IVehicleRepository vehicleRepository) => _vehicleRepository = vehicleRepository;

    public async Task RegisterAsync(CreateVehicleRequest dto)
    {
        var vehicle = Vehicle.Create(dto);
        await _vehicleRepository.AddAsync(vehicle);
    }

    public async Task<Vehicle> GetByIdAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        return vehicle ?? throw new InvalidVehicleException(id);
    }

    public Task<IEnumerable<Vehicle>> SearchAsync(VehicleSearchQuery q)
        => _vehicleRepository.SearchAsync(q.Type, q.Manufacturer, q.Model, q.Year);
}