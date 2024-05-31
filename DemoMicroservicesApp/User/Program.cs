using Microsoft.EntityFrameworkCore;
using User;
using User.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserConnectionString"));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// run migrations
using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetService<UserDbContext>();

    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
    var pendingMigrationsCount = pendingMigrations.Count();

    if (pendingMigrationsCount > 0)
    {
        Console.WriteLine($"Applying [{pendingMigrationsCount}] pending migrations:" + string.Join(Environment.NewLine, pendingMigrations));

        await dbContext.Database.MigrateAsync();

        Console.WriteLine($"SUCCESS: Database is updated. Migrations were successfully applied");
    }
    else
    {
        Console.WriteLine("INFO: No pending migrations");
    }

    var lastAppliedMigration = (await dbContext.Database.GetAppliedMigrationsAsync()).Last();
    Console.WriteLine($"Schema version: {lastAppliedMigration}");
}

app.Run();
