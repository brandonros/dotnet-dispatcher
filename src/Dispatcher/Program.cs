using Dispatcher.Extensions;
using MassTransit;
using Dispatcher.Model.Requests;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.RegisterServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, config) =>
    {
        var uri = new Uri(Environment.GetEnvironmentVariable("RABBITMQ_URI") ?? null);
        config.Host(uri, h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? null);
            h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? null);
        });
    });
    x.AddRequestClient<GetUserRequest>(new Uri("queue:q.user.get"), RequestTimeout.After(s: 5));
});

// Add logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
