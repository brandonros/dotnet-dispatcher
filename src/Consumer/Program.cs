using Consumer.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterServices();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var app = builder.Build();
app.Run();
