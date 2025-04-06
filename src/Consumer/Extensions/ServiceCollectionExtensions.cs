using Consumer.Handlers;
using Common.Telemetry;
using MassTransit;

namespace Consumer.Extensions;

// Consumer
public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterTelemetry(configuration);
        services.AddMassTransit(x =>
        {
            // Register consumers to handle JsonRpcRequest wrappers
            x.AddConsumer<GetUserJsonRpcRequestHandler>();
            x.AddConsumer<GetAccountJsonRpcRequestHandler>();
            
            x.UsingRabbitMq((context, config) =>
            {
                var rabbitConfig = configuration.GetSection("RabbitMQ");
                
                var uri = new Uri(rabbitConfig["Uri"]);
                config.Host(uri, "/", h =>
                {
                    h.Username(rabbitConfig["Username"]);
                    h.Password(rabbitConfig["Password"]);
                });
                
                // Configure endpoints for JsonRpcRequest wrapped requests
                config.ReceiveEndpoint("q.user.get", e =>
                {
                    e.Durable = true;
                    e.Bind("x.user.get");
                    e.ConfigureConsumer<GetUserJsonRpcRequestHandler>(context);
                });
                
                // Add the account endpoint
                config.ReceiveEndpoint("q.account.get", e =>
                {
                    e.Durable = true;
                    e.Bind("x.account.get");
                    e.ConfigureConsumer<GetAccountJsonRpcRequestHandler>(context);
                });
                
                config.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}