using MassTransit;
using Messages;

namespace Inventory.Consumers;

public class CreatedOrderEventConsumer : IConsumer<CreatedOrderEvent>
{
    public Task Consume(ConsumeContext<CreatedOrderEvent> context)
    {
        Console.WriteLine("CreatedOrderEvent consumed!");

        return Task.CompletedTask;
    }
}
