using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace AuctionService.Conrtollers;

[ApiController] // Checks for validity of the request, things like all required properties are added,
// and binds the incoming parameters to the properties of the request object.
[Route("api/auctions")]
public class AuctionController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint publishEndpoint;

    public AuctionController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        this._context = context;
        this._mapper = mapper;
        this.publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();
        if (!string.IsNullOrWhiteSpace(date))
        {
            // Update the query to include auctions that are updated after the input date parameter
            query = query.Where(x => x.UpdatedAt > DateTime.Parse(date).ToUniversalTime());
            // query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }
        
        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<AuctionDto>(auction));
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);
        // TODO: Add current user as seller
        auction.Seller = "test";
        _context.Auctions.Add(auction);
        var newAuction = _mapper.Map<AuctionDto>(auction);
        await publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));
        var result = await _context.SaveChangesAsync() > 0;
        if (!result)
        {
            return BadRequest("Failed to create auction");
        }

        return CreatedAtAction(nameof(GetAuctionById),
         new { id = auction.Id },
          newAuction); // This returns a Created At Action for the client to know where to find the created resource
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto auctionDto)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null)
        {
            return NotFound();
        }

        // TODO: Check if the current user is the seller
        
        auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = auctionDto.Year ?? auction.Item.Year; // ?? operator works here because the properties are nullable, eventhough they are int.

        var auctionUpdated = _mapper.Map<AuctionUpdated>(auction);
        await publishEndpoint.Publish(auctionUpdated);
        var result = await _context.SaveChangesAsync() > 0;
        if (!result)
        {
            return BadRequest("Failed to update auction");
        }
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null)
        {
            return NotFound();
        }
        // TODO: Check if the current user is the seller
        _context.Auctions.Remove(auction);
        AuctionDeleted auctionDeleted = new()
        {
            Id = id.ToString()
        };
        await publishEndpoint.Publish(auctionDeleted);
        var result = await _context.SaveChangesAsync() > 0;
        if (!result)
        {
            return BadRequest("Failed to delete auction");
        }
        return Ok();
    }
}
