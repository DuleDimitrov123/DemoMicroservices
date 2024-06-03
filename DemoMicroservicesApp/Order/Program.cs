using MongoDB.Driver;
using Order.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMongoClient>(s =>
{
    return new MongoClient(builder.Configuration.GetConnectionString("MongoDb"));
});
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

//CAP
builder.Services.AddCap(options =>
{
    options.UseMongoDB(mongoDBOptions =>
    {
        mongoDBOptions.DatabaseConnection = builder.Configuration.GetConnectionString("MongoDb");
    });

    options.UseRabbitMQ(rabbitMQOptions =>
    {
        rabbitMQOptions.ConnectionFactoryOptions = factory =>
        {
            factory.Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ"));
        };
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
