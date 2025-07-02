using System.Collections.Concurrent;
using AuctionApi.Exceptions;
using AuctionApi.Models;

namespace AuctionApi.Repositories;

public class InMemoryVehicleRepository : IVehicleRepository
{
    private readonly ConcurrentDictionary<Guid, Vehicle> _vehicles = new();

    public Task AddAsync(Vehicle vehicle)
    {
        if (!_vehicles.TryAdd(vehicle.Id, vehicle))
            throw new DuplicateVehicleException(vehicle.Id);

        return Task.CompletedTask;
    }

    public Task<Vehicle?> GetByIdAsync(Guid id)
    {
        _vehicles.TryGetValue(id, out var vehicle);
        return Task.FromResult(vehicle);
    }

    public Task<IEnumerable<Vehicle>> SearchAsync(VehicleTypeDto? type, string? manufacturer, string? model, int? year)
    {
        var q = _vehicles.Values.AsEnumerable();

        if (type.HasValue)
            q = q.Where(v => v.Type == type.Value);

        if (!string.IsNullOrWhiteSpace(manufacturer))
            q = q.Where(v => v.Manufacturer.Equals(manufacturer, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(model))
            q = q.Where(v => v.Model.Equals(model, StringComparison.OrdinalIgnoreCase));
            
        if (year.HasValue)
            q = q.Where(v => v.Year == year.Value);

        return Task.FromResult(q);
    }
}
