using Microsoft.EntityFrameworkCore;
using WebAppPlayground.Data;
using WebAppPlayground;
using WebAppPlayground.BasicRabbitMq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<JsonOwnedContext>((optionsBuilder) =>
{
    var dbConnectionString = builder.Configuration.GetConnectionString("Database");

    optionsBuilder
        .UseNpgsql(dbConnectionString)
        .UseSnakeCaseNamingConvention();
});

// Add RabbitMQ background services
builder.Services.AddHostedService<DemoRabbitMqPublisherBackgroundService>();
builder.Services.AddHostedService<DemoRabbitMqConsumerBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var jsonOwnedContext = scope.ServiceProvider.GetRequiredService<JsonOwnedContext>();
    jsonOwnedContext.Database.Migrate();
}

app.UseHttpsRedirection();
app.MapJsonOwnerEndpoints();

app.Run();