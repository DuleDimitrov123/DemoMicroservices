using Inventory.Consumers;
using MassTransit;
using Messages;

namespace Inventory.Extensions;

public static class RegisterServices
{
    public static void AddCustomCap(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ISubscriberService, SubscriberService>();

        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMqConfig").Get<RabbitMqConfig>();

        builder.Services.Configure<RabbitMqConfig>(options =>
            builder.Configuration.GetSection("RabbitMqConfig").Bind(options));

        builder.Services.AddCap(options =>
        {
            options.UseEntityFramework<InventoryDbContext>();

            options.UseRabbitMQ(options =>
            {
                options.ExchangeName = rabbitMqConfig.ExchangeName;

                options.ConnectionFactoryOptions = factory =>
                {
                    factory.Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ"));
                };
            });
        });
    }

    public static void AddCustomMassTransit(this WebApplicationBuilder builder)
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMqConfig").Get<RabbitMqConfig>();

        builder.Services.Configure<RabbitMqConfig>(options =>
            builder.Configuration.GetSection("RabbitMqConfig").Bind(options));

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

                    c.Bind(rabbitMqConfig.ExchangeName, x =>
                    {
                        x.ExchangeType = rabbitMqConfig.ExchangeType; // default is fanout
                        x.RoutingKey = rabbitMqConfig.RoutingKey;
                    });
                });
            });
        });

        builder.Services.AddScoped<CreatedOrderEventConsumer>();
    }
}
