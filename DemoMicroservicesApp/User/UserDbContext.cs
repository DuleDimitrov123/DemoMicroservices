using Microsoft.EntityFrameworkCore;

namespace User;

public class UserDbContext : DbContext
{
    public DbSet<Entities.User> Users { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {

    }
}
