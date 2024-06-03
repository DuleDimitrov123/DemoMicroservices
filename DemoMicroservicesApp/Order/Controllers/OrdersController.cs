using Microsoft.AspNetCore.Mvc;
using Order.Controllers.Requests;
using Order.Controllers.Responses;
using Order.Repositories;

namespace Order.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;

    public OrdersController(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IList<GetOrderResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAll(cancellationToken);

        return Ok(orders.Select(o =>
            new GetOrderResponse(
                o.Id,
                o.Products.Select(p => new GetProductResponse(p.Name)).ToList(),
                o.Username)));
    }

    [HttpPost]
    public async Task<ActionResult<string>> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var id = await _orderRepository.Create(
            new Entities.Order()
            {
                Products = request.Products.Select(p => new Entities.Product() { Name = p.Name, ProductServiceId = p.ProductServiceId }).ToList(),
                Username = request.Username,
                UserServiceId = request.UserServiceId
            },
            cancellationToken);

        return Ok(id);
    }
}
