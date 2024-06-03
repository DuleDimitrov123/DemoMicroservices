using DotNetCore.CAP;
using Inventory.Controllers.Requests;
using Inventory.Controllers.Responses;
using Inventory.Entities;
using Inventory.Repositories;
using Messages;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var id = await _productRepository.Create(
            new Product()
            {
                Name = request.Name,
                Description = request.Description,
                Quantity = request.Quantity,
            },
            cancellationToken);

        return Ok(id);
    }

    [HttpGet]
    public async Task<ActionResult<IList<GetProductResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAll(cancellationToken);

        return Ok(
            products
                .Select(p =>
                    new GetProductResponse(p.Id, p.Name, p.Description, p.Quantity)));
    }

    [HttpPost("CAPROUTE-createorder")]
    [CapSubscribe("order.created")]
    public async Task<ActionResult> CreateOrder(CreatedOrderEvent createdOrderEvent)
    {
        return Ok();
    }
}
