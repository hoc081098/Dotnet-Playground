using Microsoft.AspNetCore.Antiforgery;
using Microsoft.EntityFrameworkCore;
using WebAppPlayground.Data;
using WebAppPlayground;
using WebAppPlayground.RabbitMq.Basic;
using WebAppPlayground.RabbitMq.TopicWithDlx;

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
DemoRabbitMq demoRabbitMq = DemoRabbitMq.TopicWithDlx;

// ReSharper disable once UnreachableSwitchCaseDueToIntegerAnalysis
switch (demoRabbitMq)
{
    case DemoRabbitMq.Basic:
        builder.Services.AddHostedService<DemoRabbitMqPublisherBackgroundService>();
        builder.Services.AddHostedService<DemoRabbitMqConsumerBackgroundService>();
        break;
    case DemoRabbitMq.TopicWithDlx:
        builder.Services.AddHostedService<TopicWithDlxRabbitMqConsumerBackgroundService>();
        builder.Services.AddHostedService<TopicWithDlxRabbitMqPublisherBackgroundService>();
        break;
    default:
        throw new InvalidOperationException($"Unknown demo RabbitMQ type: {demoRabbitMq}");
}

builder.Services.AddAntiforgery(options =>
{
    // Just setting the name of XSRF token
    options.HeaderName = "X-XSRF-TOKEN";
});

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
app.UseAntiforgery();

// Get token
app.MapGet("antiforgery/token", (IAntiforgery forgeryService, HttpContext context) =>
{
    var tokens = forgeryService.GetAndStoreTokens(context);
    var xsrfToken = tokens.RequestToken!;
    return TypedResults.Content(xsrfToken, contentType: "text/plain");
});

app.MapJsonOwnerEndpoints();
app.MapDemoRestApiParamsEndPoints1();
app.MapDemoRestApiParamsEndPoints2();

app.Run();

enum DemoRabbitMq
{
    Basic,
    TopicWithDlx
}