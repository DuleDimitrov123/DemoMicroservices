using MassTransit;

namespace Order.Extensions;

public static class RegisterServices
{
    public static void AddCustomCap(this WebApplicationBuilder builder)
    {
        builder.Services.AddCap(options =>
        {
            options.UseMongoDB(mongoDBOptions =>
            {
                mongoDBOptions.DatabaseConnection = builder.Configuration.GetConnectionString("MongoDb");
            });

            options.UseRabbitMQ(rabbitMQOptions =>
            {
                rabbitMQOptions.ExchangeName = "order-created-topic-exchange";

                rabbitMQOptions.ConnectionFactoryOptions = factory =>
                {
                    factory.Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ"));
                };
            });
        });
    }

    public static void AddCustomMassTransit(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((ctx, cfg) =>
            {
                var uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ"));

                cfg.Host(uri, h =>
                {
                    h.Username(uri.UserInfo.Split(':')[0]);
                    h.Password(uri.UserInfo.Split(':')[1]);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });
    }
}
