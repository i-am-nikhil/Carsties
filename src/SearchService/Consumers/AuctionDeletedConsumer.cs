using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    private IMapper _mapper;
    public AuctionDeletedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        System.Console.WriteLine("--> Consuming Auction Deleted: " + context.Message.Id);
       var ressult = await DB.DeleteAsync<Item>(context.Message.Id);
        if (!ressult.IsAcknowledged)
            throw new MessageException(typeof(AuctionDeleted), "Failed to delete item");
    }
}
