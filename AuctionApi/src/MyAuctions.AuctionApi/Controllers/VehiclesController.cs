using AuctionApi.Dtos;
using AuctionApi.Exceptions;
using AuctionApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Controllers;

[ApiController]
[Route("[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    public VehiclesController(IVehicleService vehicleService) => _vehicleService = vehicleService;

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateVehicleRequest dto)
    {
        try
        {
            await _vehicleService.RegisterAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, null);
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DuplicateVehicleException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);
            return Ok(vehicle);
        }
        catch (InvalidVehicleException)
        {
            return NotFound();
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] VehicleSearchQuery query)
    {
        var results = await _vehicleService.SearchAsync(query);
        return Ok(results);
    }
}