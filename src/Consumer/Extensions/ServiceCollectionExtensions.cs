using Consumer.Handlers;
using MassTransit;
using Common.Model.Requests;
using Common.Model;
using Microsoft.Extensions.Configuration;

namespace Consumer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        // Configure MassTransit
        services.AddMassTransit(x =>
        {
            x.AddConsumer<GetUserHandler>();
            x.UsingRabbitMq((context, config) =>
            {
                var configuration = context.GetRequiredService<IConfiguration>();
                var rabbitConfig = configuration.GetSection("RabbitMQ");
                
                var uri = new Uri(rabbitConfig["Uri"]);
                config.Host(uri, "/", h =>
                {
                    h.Username(rabbitConfig["Username"]);
                    h.Password(rabbitConfig["Password"]);
                });
                
                config.ReceiveEndpoint("q.user.get", e =>
                {
                    e.Bind("x.user.get");
                    e.ConfigureConsumer<GetUserHandler>(context);
                });
                config.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
