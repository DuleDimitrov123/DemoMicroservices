using Inventory;
using Inventory.Consumers;
using Inventory.Repositories;
using MassTransit;
using Messages;
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
//builder.Services.AddCap(options =>
//{
//    options.UseEntityFramework<InventoryDbContext>();

//    options.UseRabbitMQ(options =>
//    {
//        options.ExchangeName = Queue.OrderCreatedQueue;

//        options.ConnectionFactoryOptions = factory =>
//        {
//            factory.Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ"));
//        };
//    });
//});

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<CreatedOrderEventConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        var uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ"));

        cfg.Host(uri, h =>
        {
            h.Username(uri.UserInfo.Split(':')[0]);
            h.Password(uri.UserInfo.Split(':')[1]);
        });

        cfg.ReceiveEndpoint(Queue.OrderCreatedQueue, c =>
        {
            c.ConfigureConsumeTopology = false; // Prevents automatic binding to default exchange
            c.ConfigureConsumer<CreatedOrderEventConsumer>(ctx);

            c.ClearSerialization();
            //c.ClearMessageDeserializers();

            c.UseRawJsonSerializer();

            c.Bind("order-created-topic-exchange", x =>
            {
                x.ExchangeType = "topic";
                x.RoutingKey = "#"; // Adjust routing key as needed
            });
        });
    });
});

builder.Services.AddScoped<CreatedOrderEventConsumer>();

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
