using Dispatcher.Services;

namespace Dispatcher.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IPingService, PingService>();
        services.AddScoped(typeof(IQueueService<,>), typeof(QueueService<,>));
        return services;
    }
}
