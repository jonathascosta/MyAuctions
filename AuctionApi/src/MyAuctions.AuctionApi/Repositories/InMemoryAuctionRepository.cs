using System.Collections.Concurrent;
using AuctionApi.Exceptions;
using AuctionApi.Models;

namespace AuctionApi.Repositories
{
    public class InMemoryAuctionRepository : IAuctionRepository
    {
        private readonly ConcurrentDictionary<Guid, Auction> _store = new();

        public Task AddAsync(Auction auction)
        {
            if (!_store.TryAdd(auction.Id, auction))
                throw new AuctionAlreadyActiveException();

            return Task.CompletedTask;
        }

        public Task<Auction?> GetActiveByVehicleAsync(Guid vehicleId)
        {
            var active = _store.Values
                .FirstOrDefault(a => a.VehicleId == vehicleId && a.EndedAt == null);

            return Task.FromResult(active);
        }

        public Task<Auction?> GetByIdAsync(Guid id)
        {
            _store.TryGetValue(id, out var auction);
            return Task.FromResult(auction);
        }

        public Task<List<Auction>> GetAllAsync()
        {
            return Task.FromResult(_store.Values.ToList());
        }

        public Task UpdateAsync(Auction auction)
        {
            // Just to simulate an update operation
            _store[auction.Id] = auction;
            return Task.CompletedTask;
        }
    }
}
