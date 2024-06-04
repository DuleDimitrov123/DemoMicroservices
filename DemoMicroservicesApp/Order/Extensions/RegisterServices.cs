using MassTransit;
using Order.Config;

namespace Order.Extensions;

public static class RegisterServices
{
    public static void AddCustomCap(this WebApplicationBuilder builder)
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMqConfig").Get<RabbitMqConfig>();

        var capConfig = builder.Configuration.GetSection("CapConfig").Get<CapConfig>();

        builder.Services.AddCap(options =>
        {
            options.FailedRetryInterval = capConfig.FailedRetryIntervalSeconds;
            options.FailedRetryCount = capConfig.FailedRetryCount;
            options.SucceedMessageExpiredAfter = capConfig.SucceedMessageExpirationSeconds;
            options.FailedMessageExpiredAfter = capConfig.FailedMessageExpirationSeconds;

            options.UseMongoDB(mongoDBOptions =>
            {
                mongoDBOptions.DatabaseConnection = builder.Configuration.GetConnectionString("MongoDb");
            });

            options.UseRabbitMQ(rabbitMQOptions =>
            {
                rabbitMQOptions.ExchangeName = rabbitMqConfig.ExchangeName;

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
