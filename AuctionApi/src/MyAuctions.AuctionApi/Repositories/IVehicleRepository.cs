using AuctionApi.Models;

namespace AuctionApi.Repositories;

public interface IVehicleRepository
{
    Task AddAsync(Vehicle vehicle);
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<IEnumerable<Vehicle>> SearchAsync(VehicleTypeDto? type, string? manufacturer, string? model, int? year);
}