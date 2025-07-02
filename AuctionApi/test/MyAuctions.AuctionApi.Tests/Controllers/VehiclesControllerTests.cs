using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AuctionApi.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using MyAuctions.AuctionApi;
using MyAuctions.AuctionApi.Tests;

namespace AuctionApi.IntegrationTests;

public class VehiclesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public VehiclesControllerTests()
    {
        _factory = new CustomWebApplicationFactory<Program>();
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenVehicleIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        var dto = new CreateVehicleRequest
        {
            Id = Guid.NewGuid(),
            Type = VehicleTypeDto.Sedan,
            Manufacturer = "Toyota",
            Model = "Camry",
            Year = 2021,
            StartingBid = 15000,
            Doors = 4
        };

        // Act
        var response = await client.PostAsJsonAsync("/Vehicles", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenDomainValidationException()
    {
        // Arrange
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        var dto = new CreateVehicleRequest
        {
            Id = Guid.NewGuid(),
            Type = VehicleTypeDto.Sedan,
            Manufacturer = "Toyota",
            Model = "Camry",
            Year = 2021,
            StartingBid = 15000
        };

        // Act
        var response = await client.PostAsJsonAsync("/Vehicles", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsConflict_WhenDuplicateVehicleException()
    {
        // Arrange
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        var dto = new CreateVehicleRequest
        {
            Id = Guid.NewGuid(),
            Type = VehicleTypeDto.Sedan,
            Manufacturer = "Toyota",
            Model = "Camry",
            Year = 2021,
            StartingBid = 15000,
            Doors = 4
        };

        // Act
        await client.PostAsJsonAsync("/Vehicles", dto);
        var response = await client.PostAsJsonAsync("/Vehicles", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsOk_WhenVehicleExists()
    {
        // Arrange
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        var dto = new CreateVehicleRequest
        {
            Id = Guid.NewGuid(),
            Type = VehicleTypeDto.Sedan,
            Manufacturer = "Toyota",
            Model = "Camry",
            Year = 2021,
            StartingBid = 15000,
            Doors = 4
        };

        // Act
        await client.PostAsJsonAsync("/Vehicles", dto);
        var response = await client.GetAsync($"/Vehicles/{dto.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenVehicleDoesNotExist()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/Vehicles/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Search_ReturnsOk_WithResults()
    {
        // Arrange
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        var dto = new CreateVehicleRequest
        {
            Id = Guid.NewGuid(),
            Type = VehicleTypeDto.Sedan,
            Manufacturer = "Toyota",
            Model = "Camry",
            Year = 2021,
            StartingBid = 15000,
            Doors = 4
        };

        // Act
        await client.PostAsJsonAsync("/Vehicles", dto);
        var response = await client.GetAsync($"/Vehicles?Model=Camry");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var vehicles = await response.Content.ReadFromJsonAsync<object[]>();
        Assert.NotNull(vehicles);
    }
}
