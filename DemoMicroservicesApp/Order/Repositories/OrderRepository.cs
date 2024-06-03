
using DotNetCore.CAP;
using MassTransit;
using Messages;
using MongoDB.Driver;

namespace Order.Repositories;

public interface IOrderRepository
{
    Task<IList<Entities.Order>> GetAll(CancellationToken cancellationToken);

    Task<string> Create(Entities.Order order, CancellationToken cancellationToken);
}

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Entities.Order> _orders;
    private readonly IMongoClient _mongoClient;
    private readonly ICapPublisher _capPublisher;
    private readonly IPublishEndpoint _publisher;

    public OrderRepository(IPublishEndpoint publisher, IMongoClient mongoClient, ICapPublisher capPublisher)
    {
        _orders = mongoClient.GetDatabase("OrderDb").GetCollection<Entities.Order>("Orders");
        _mongoClient = mongoClient;
        _capPublisher = capPublisher;
        _publisher = publisher;
    }

    public async Task<string> Create(Entities.Order order, CancellationToken cancellationToken)
    {
        await _orders.InsertOneAsync(order, cancellationToken);

        //await _publisher.Publish(new CreatedOrderEvent()
        //{
        //    Products = order.Products.Select(p => new ProductEvent() { Name = p.Name, ProductServiceId = p.ProductServiceId }).ToList(),
        //    Username = order.Username,
        //    UserServiceId = order.UserServiceId
        //});

        await _capPublisher.PublishAsync(Queue.OrderCreatedQueue,
            new CreatedOrderEvent()
            {
                Products = order.Products.Select(p => new ProductEvent() { Name = p.Name, ProductServiceId = p.ProductServiceId }).ToList(),
                Username = order.Username,
                UserServiceId = order.UserServiceId
            });

        return order.Id;

        //using (var session = await _mongoClient.StartSessionAsync())
        //{
        //    session.StartTransaction();

        //    try
        //    {
        //        await _orders.InsertOneAsync(session, order, cancellationToken: cancellationToken);

        //        await _capPublisher.PublishAsync("order.created",
        //            new CreatedOrderEvent()
        //            {
        //                Products = order.Products.Select(p => new ProductEvent() { Name = p.Name, ProductServiceId = p.ProductServiceId }).ToList(),
        //                Username = order.Username,
        //                UserServiceId = order.UserServiceId
        //            });

        //        await session.CommitTransactionAsync();

        //        return order.Id;
        //    }
        //    catch
        //    {
        //        await session.AbortTransactionAsync();
        //        throw;
        //    }
        //}


        //using (var session = _mongoClient.StartTransaction(_capPublisher, autoCommit: false))
        //{
        //    await _orders.InsertOneAsync(order, cancellationToken);

        //    await _capPublisher.PublishAsync("order.created",
        //        new CreatedOrderEvent()
        //        {
        //            Products = order.Products.Select(p => new ProductEvent() { Name = p.Name, ProductServiceId = p.ProductServiceId }).ToList(),
        //            Username = order.Username,
        //            UserServiceId = order.UserServiceId
        //        });

        //    await session.CommitTransactionAsync(cancellationToken);

        //    return order.Id;
        //}
    }

    public async Task<IList<Entities.Order>> GetAll(CancellationToken cancellationToken)
    {
        return await _orders.Find(order => true).ToListAsync(cancellationToken);
    }
}
