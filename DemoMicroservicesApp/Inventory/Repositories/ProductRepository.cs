using Inventory.Entities;
using Messages;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repositories;

public interface IProductRepository
{
    Task<int> Create(Product product, CancellationToken cancellationToken);

    Task<IList<Product>> GetAll(CancellationToken cancellationToken);

    Task Update(IList<ProductEvent> productEvents, CancellationToken cancellationToken);
}

public class ProductRepository : IProductRepository
{
    private readonly InventoryDbContext _dbContext;

    public ProductRepository(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Create(Product product, CancellationToken cancellationToken)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return product.Id;
    }

    public async Task<IList<Product>> GetAll(CancellationToken cancellationToken)
    {
        return await _dbContext.Products.ToListAsync(cancellationToken);
    }

    public async Task Update(IList<ProductEvent> productEvents, CancellationToken cancellationToken)
    {
        foreach (var productEvent in productEvents)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productEvent.ProductServiceId, cancellationToken);

            if (product != null)
            {
                product.Quantity--;
                _dbContext.Products.Update(product);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
