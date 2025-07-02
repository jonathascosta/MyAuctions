using AuctionApi.Dtos;
using AuctionApi.Exceptions;
using AuctionApi.Models;
using AuctionApi.Repositories;
using AuctionApi.Services;
using Moq;

namespace MyAuctions.AuctionApi.Tests.Services;

public class AuctionServiceTests
{
    private readonly Mock<IAuctionRepository> _auctionRepoMock;
    private readonly Mock<IVehicleRepository> _vehicleRepoMock;
    private readonly AuctionService _service;

    public AuctionServiceTests()
    {
        _auctionRepoMock = new Mock<IAuctionRepository>();
        _vehicleRepoMock = new Mock<IVehicleRepository>();
        _service = new AuctionService(_auctionRepoMock.Object, _vehicleRepoMock.Object);
    }

    private (Guid vehicleId, Vehicle vehicle) CreateVehicle()
    {
        var vehicleId = Guid.NewGuid();
        var dto = new CreateVehicleRequest
        {
            Id = vehicleId,
            Manufacturer = "Toyota",
            Model = "Camry",
            Year = 2021,
            Doors = 4,
            StartingBid = 15000
        };
        var vehicle = Sedan.Create(dto);
        return (vehicleId, vehicle);
    }

    [Fact]
    public async Task StartAsync_ShouldStartAuction_WhenVehicleExistsAndNoActiveAuction()
    {
        // Arrange
        Auction? createdAuction = null;
        var (vehicleId, vehicle) = CreateVehicle();

        _vehicleRepoMock.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);

        _auctionRepoMock.Setup(r => r.GetActiveByVehicleAsync(vehicleId))
            .ReturnsAsync((Auction?)null);

        _auctionRepoMock.Setup(r => r.AddAsync(It.IsAny<Auction>()))
            .Callback<Auction>(auction => createdAuction = auction)
            .Returns(Task.CompletedTask);

        // Act
        var id = await _service.StartAsync(vehicleId, 100);

        // Assert
        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(createdAuction);
        Assert.Equal(id, createdAuction.Id);
    }

    [Fact]
    public async Task StartAsync_ShouldThrowInvalidVehicleException_WhenVehicleNotFound()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();

        _vehicleRepoMock.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync((Vehicle?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidVehicleException>(() => _service.StartAsync(vehicleId, 100));
    }

    [Fact]
    public async Task StartAsync_ShouldThrowAuctionAlreadyActiveException_WhenActiveAuctionExists()
    {
        // Arrange
        var (vehicleId, vehicle) = CreateVehicle();

        _vehicleRepoMock.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);

        _auctionRepoMock.Setup(r => r.GetActiveByVehicleAsync(vehicleId))
            .ReturnsAsync(Auction.Create(vehicleId, vehicle.StartingBid));

        // Act & Assert
        await Assert.ThrowsAsync<AuctionAlreadyActiveException>(() => _service.StartAsync(vehicleId, 100));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAuction_WhenFound()
    {
        // Arrange
        var (vehicleId, vehicle) = CreateVehicle();
        var auction = Auction.Create(vehicleId, vehicle.StartingBid);

        _auctionRepoMock.Setup(r => r.GetByIdAsync(auction.Id))
            .ReturnsAsync(auction);

        // Act
        var result = await _service.GetByIdAsync(auction.Id);

        // Assert
        Assert.Equal(auction, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowAuctionNotFoundException_WhenNotFound()
    {
        // Arrange
        var auctionId = Guid.NewGuid();

        _auctionRepoMock.Setup(r => r.GetByIdAsync(auctionId))
            .ReturnsAsync((Auction?)null);

        // Act & Assert
        await Assert.ThrowsAsync<AuctionNotFoundException>(() => _service.GetByIdAsync(auctionId));
    }

    [Fact]
    public async Task PlaceBidAsync_ShouldPlaceBidAndUpdateAuction_WhenFound()
    {
        // Arrange
        var (vehicleId, vehicle) = CreateVehicle();
        var auction = Auction.Create(vehicleId, vehicle.StartingBid);

        _auctionRepoMock.Setup(r => r.GetByIdAsync(auction.Id))
            .ReturnsAsync(auction);

        Auction? updatedAuction = null;
        _auctionRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Auction>()))
            .Callback<Auction>(a => updatedAuction = a)
            .Returns(Task.CompletedTask);

        // Act
        await _service.PlaceBidAsync(auction.Id, "bidder", 20000);

        // Assert
        Assert.Equal(auction.Id, updatedAuction!.Id);
        Assert.Equal("bidder", auction.Bids.Last().Bidder);
        Assert.Equal(20000, auction.Bids.Last().Amount);
    }

    [Fact]
    public async Task PlaceBidAsync_ShouldThrowAuctionNotFoundException_WhenNotFound()
    {
        // Arrange
        var auctionId = Guid.NewGuid();

        _auctionRepoMock.Setup(r => r.GetByIdAsync(auctionId))
            .ReturnsAsync((Auction?)null);

        // Act & Assert
        await Assert.ThrowsAsync<AuctionNotFoundException>(() => _service.PlaceBidAsync(auctionId, "bidder", 200));
    }

    [Fact]
    public async Task StopAsync_ShouldCloseAndUpdateAuction_WhenFound()
    {
        // Arrange
        var (vehicleId, vehicle) = CreateVehicle();
        var auction = Auction.Create(vehicleId, vehicle.StartingBid);

        _auctionRepoMock.Setup(r => r.GetByIdAsync(auction.Id))
            .ReturnsAsync(auction);

        Auction? updatedAuction = null;
        _auctionRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Auction>()))
            .Callback<Auction>(a => updatedAuction = a)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.StopAsync(auction.Id);

        // Assert
        Assert.Equal(auction.Id, result.Id);
        Assert.NotNull(result.EndedAt);
    }

    [Fact]
    public async Task StopAsync_ShouldThrowAuctionNotFoundException_WhenNotFound()
    {
        // Arrange
        var auctionId = Guid.NewGuid();

        _auctionRepoMock.Setup(r => r.GetByIdAsync(auctionId))
            .ReturnsAsync((Auction?)null);

        // Act & Assert
        await Assert.ThrowsAsync<AuctionNotFoundException>(() => _service.StopAsync(auctionId));
    }
}
