using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

namespace Common.Telemetry;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterTelemetry(this IServiceCollection services)
    {
        var serviceName = Assembly.GetExecutingAssembly().GetName().Name!;
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(e => {
                    e.Endpoint = new Uri("http://localhost:4318/v1/traces");
                    e.Protocol = OtlpExportProtocol.HttpProtobuf;
                }))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(e => {
                    e.Endpoint = new Uri("http://localhost:4318/v1/metrics");
                    e.Protocol = OtlpExportProtocol.HttpProtobuf;
                }));
        return services;
    }
}
