using Common.Model.Requests;
using Dispatcher.Services;
using MassTransit;
namespace Dispatcher.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IQueueService<,>), typeof(QueueService<,>));
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, config) =>
            {
                var uri = new Uri(Environment.GetEnvironmentVariable("RABBITMQ_URI") ?? null);
                config.Host(uri, "/", h =>
                {
                    h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? null);
                    h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? null);
                });
            });
            x.AddRequestClient<GetUserRequest>(new Uri("queue:q.user.get"), RequestTimeout.After(s: 5));
        });
        return services;
    }
}
