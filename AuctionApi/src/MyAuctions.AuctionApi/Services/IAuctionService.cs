using AuctionApi.Models;

namespace AuctionApi.Services;

public interface IAuctionService
{
    Task<Guid> StartAsync(Guid vehicleId, decimal startingBid);
    Task<Auction> GetByIdAsync(Guid id);
    Task<List<Auction>> GetAllAsync();
    Task PlaceBidAsync(Guid auctionId, string bidder, decimal amount);
}
