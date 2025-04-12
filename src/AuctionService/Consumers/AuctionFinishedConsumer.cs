using AuctionService.Data;
using AuctionService.Entities;
using Contracts;
using MassTransit;

namespace AuctionService;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    private AuctionDbContext _context;

    public AuctionFinishedConsumer(AuctionDbContext dbContext)
    {
        _context = dbContext;
    }
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        System.Console.WriteLine("--> Consuming auction finished");
        var auction = await _context.Auctions.FindAsync(context.Message.AuctionId);
        if (context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = context.Message.Amount;
        }
        
        auction.Status = auction.SoldAmount > auction.ReservePrice ? Status.Finished : Status.ReserveNotMet;

        await _context.SaveChangesAsync();
    }
}
