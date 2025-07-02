using AuctionApi.Exceptions;
using AuctionApi.Models;
using AuctionApi.Repositories;

namespace AuctionApi.Services;

public class AuctionService(IAuctionRepository auctionRepository, IVehicleRepository vehicleRepository) : IAuctionService
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

    public async Task<Guid> StartAsync(Guid vehicleId, decimal startingBid)
    {
        _ = await _vehicleRepository.GetByIdAsync(vehicleId) ?? throw new InvalidVehicleException(vehicleId);
        var existing = await _auctionRepository.GetActiveByVehicleAsync(vehicleId);
        if (existing != null)
            throw new AuctionAlreadyActiveException();

        var auction = Auction.Create(vehicleId, startingBid);
        await _auctionRepository.AddAsync(auction);
        return auction.Id;
    }

    public async Task<Auction> GetByIdAsync(Guid id)
    {
        var auction = await _auctionRepository.GetByIdAsync(id) ?? throw new AuctionNotFoundException();
        return auction;
    }

    public async Task<List<Auction>> GetAllAsync()
    {
        var auctions = await _auctionRepository.GetAllAsync();

        return auctions;
    }

    public async Task<Auction> StopAsync(Guid id)
    {
        var auction = await _auctionRepository.GetByIdAsync(id) ?? throw new AuctionNotFoundException();

        auction.Close();
        await _auctionRepository.UpdateAsync(auction);
        return auction;
    }

    public async Task PlaceBidAsync(Guid auctionId, string bidder, decimal amount)
    {
        var auction = await _auctionRepository.GetByIdAsync(auctionId)
                      ?? throw new AuctionNotFoundException();

        auction.PlaceBid(bidder, amount); 

        await _auctionRepository.UpdateAsync(auction);
    }
}
