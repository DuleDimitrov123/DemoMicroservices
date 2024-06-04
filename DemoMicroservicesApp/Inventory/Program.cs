using Inventory;
using Inventory.Consumers;
using Inventory.Extensions;
using Inventory.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("InventoryConnectionString"));
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();

// CAP
//builder.AddCustomCap();

builder.AddCustomMassTransit();

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
    var dbContext = serviceScope.ServiceProvider.GetService<InventoryDbContext>();

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
