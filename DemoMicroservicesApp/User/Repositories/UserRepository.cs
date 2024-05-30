
using Microsoft.EntityFrameworkCore;

namespace User.Repositories;

public interface IUserRepository
{
    Task<int> Create(Entities.User user, CancellationToken cancellationToken);

    Task<IList<Entities.User>> GetAll(CancellationToken cancellationToken);
}

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;

    public UserRepository(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Create(Entities.User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    public async Task<IList<Entities.User>> GetAll(CancellationToken cancellationToken)
    {
        return await _dbContext.Users.ToListAsync(cancellationToken);
    }
}
