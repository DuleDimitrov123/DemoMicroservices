using Inventory.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory;

public class InventoryDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {

    }
}
