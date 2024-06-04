using Inventory.Repositories;
using MassTransit;
using Messages;

namespace Inventory.Consumers;

public class CreatedOrderEventConsumer : IConsumer<CreatedOrderEvent>
{
    private readonly IProductRepository _productRepository;

    public CreatedOrderEventConsumer(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task Consume(ConsumeContext<CreatedOrderEvent> context)
    {
        await _productRepository.Update(context.Message.Products, context.CancellationToken);
    }
}
