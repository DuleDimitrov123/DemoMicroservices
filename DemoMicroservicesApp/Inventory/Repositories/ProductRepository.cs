using Inventory.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repositories;

public interface IProductRepository
{
    Task<int> Create(Product product, CancellationToken cancellationToken);

    Task<IList<Product>> GetAll(CancellationToken cancellationToken);
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
}
