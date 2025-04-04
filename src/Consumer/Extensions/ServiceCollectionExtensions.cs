using Consumer.Handlers;
using MassTransit;
using Common.Model.Requests;
using Common.Model;

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
                var uri = new Uri(Environment.GetEnvironmentVariable("RABBITMQ_URI") ?? null);
                config.Host(uri, "/", h =>
                {
                    h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? null);
                    h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? null);
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
