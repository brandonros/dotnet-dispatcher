using Consumer.Handlers;
using Consumer.Services;
using MassTransit;
using Common.Model;
using Common.Model.Requests;

namespace Consumer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        // Register your handlers
        services.AddScoped<GetUserHandler>();

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
                config.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
