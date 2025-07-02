using AuctionApi.Models;

namespace AuctionApi.Repositories;

public interface IAuctionRepository
{
    Task AddAsync(Auction auction);
    Task<Auction?> GetActiveByVehicleAsync(Guid vehicleId);
    Task<Auction?> GetByIdAsync(Guid id);
    Task<List<Auction>> GetAllAsync();
    Task UpdateAsync(Auction auction);
}
