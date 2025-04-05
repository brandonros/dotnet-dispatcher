using Common.Model.Requests;
using Dispatcher.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace Dispatcher.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IQueueService<,>), typeof(QueueService<,>));
        services.AddMassTransit(x =>
        {
            x.AddRequestClient<GetUserRequest>(new Uri("exchange:x.user.get"), RequestTimeout.After(s: 5));
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
                config.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}
