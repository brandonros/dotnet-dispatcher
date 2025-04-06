using Dispatcher.Services;
using MassTransit;
using Common.Model.Requests;
using Common.Model.JsonRpc;
using Common.Telemetry;

namespace Dispatcher.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterTelemetry(configuration);
        services.AddScoped(typeof(IQueueService<,>), typeof(QueueService<,>));
        services.AddMassTransit(x =>
        {
            // Change this to use JsonRpcRequest<GetUserRequest> instead of just GetUserRequest
            x.AddRequestClient<JsonRpcRequest<GetUserRequest>>(new Uri("exchange:x.user.get"), RequestTimeout.After(s: 5));
            
            // Add the account request client too
            x.AddRequestClient<JsonRpcRequest<GetAccountRequest>>(new Uri("exchange:x.account.get"), RequestTimeout.After(s: 5));
            
            x.UsingRabbitMq((context, config) =>
            {
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
