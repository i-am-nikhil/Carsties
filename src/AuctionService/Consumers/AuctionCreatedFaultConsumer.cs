using Contracts;
using MassTransit;

namespace AuctionService;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        System.Console.WriteLine($"--> Consuming faulty creation: {context.Message.Message}");
        var exception = context.Message.Exceptions.First();
        if (exception.ExceptionType == "System.ArgumentException")
        {
            context.Message.Message.Model = "FooBar"; // First message is the Fault, second message is the original message
            await context.Publish(context.Message.Message);
        }
        else
        {
            System.Console.WriteLine("Update error dashboard...");
        }
    }
}
