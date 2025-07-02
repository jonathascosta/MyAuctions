using AuctionApi.Dtos;
using AuctionApi.Models;

namespace AuctionApi.Services;

public interface IVehicleService
{
    Task RegisterAsync(CreateVehicleRequest dto);
    Task<Vehicle> GetByIdAsync(Guid id);
    Task<IEnumerable<Vehicle>> SearchAsync(VehicleSearchQuery query);
}