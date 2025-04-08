using Yarp.ReverseProxy.Swagger.Extensions;
using Yarp.ReverseProxy.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthProxy.Config;
using Microsoft.Extensions.Options;
using AuthProxy.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration")))
    };
});

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());
});

// Get the ReverseProxy configuration section
var configuration = builder.Configuration.GetSection("ReverseProxy");

// Add reverse proxy capabilities with Swagger
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(configuration)
    .AddSwagger(configuration);

// Add Swagger services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(o =>
    {
        //var config = app.Services.GetRequiredService<IOptionsMonitor<ReverseProxyDocumentFilterConfig>>().CurrentValue;
        //o.ConfigureSwaggerEndpoints(config);
    });

// Enable endpoint routing and the reverse proxy
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();

app.Run();