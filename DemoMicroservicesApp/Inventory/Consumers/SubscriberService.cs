using DotNetCore.CAP;
using Inventory.Repositories;
using Messages;

namespace Inventory.Consumers;

public interface ISubscriberService
{
    Task Handle(CreatedOrderEvent createdOrderEvent);
}

public class SubscriberService : ISubscriberService, ICapSubscribe
{
    private readonly IProductRepository _productRepository;

    public SubscriberService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [CapSubscribe(Queue.OrderCreatedQueue)]
    public async Task Handle(CreatedOrderEvent createdOrderEvent)
    {
        await _productRepository.Update(createdOrderEvent.Products, CancellationToken.None);
    }
}
