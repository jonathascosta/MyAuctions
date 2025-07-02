using System.Security.Claims;
using AuctionApi.Dtos;
using AuctionApi.Exceptions;
using AuctionApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAuctions.AuctionApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuctionsController(IAuctionService auctionService) : Controller
{
    private readonly IAuctionService _auctionService = auctionService;

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Start([FromBody] StartAuctionDto dto)
    {
        try
        {
            var auctionId = await _auctionService.StartAsync(dto.VehicleId, dto.StartingBid);
            return CreatedAtAction(nameof(GetById), new { id = auctionId }, null);
        }
        catch (InvalidVehicleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (AuctionAlreadyActiveException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var auction = await _auctionService.GetByIdAsync(id);
            return Ok(auction);
        }
        catch (AuctionNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var auctions = await _auctionService.GetAllAsync();
        return Ok(auctions);
    }

    [HttpPost("{id}/bids")]
    [Authorize(Policy = "BidderOnly")]
    public async Task<IActionResult> PlaceBid(
        Guid id,
        [FromBody] PlaceBidDto dto)
    {
        var bidder = User.FindFirstValue(ClaimTypes.Name)!;

        try
        {
            await _auctionService.PlaceBidAsync(id, bidder, dto.Amount);
            return NoContent();
        }
        catch (AuctionNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (AuctionNotActiveException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidBidException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
