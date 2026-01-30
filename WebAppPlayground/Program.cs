using Microsoft.EntityFrameworkCore;
using WebAppPlayground.Data;
using WebAppPlayground;

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

builder.Services.AddHostedService<DemoRabbitMqBackgroundService>();
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

app.MapGet("/json-owner/{id:int}",
    async (int id,
        JsonOwnedContext dbContext,
        CancellationToken cancellationToken) =>
    {
        var item = await dbContext.JsonOwners
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return item is null ? Results.NotFound() : Results.Ok(item);
    });

app.MapPut("/json-owner/{id:int}",
    async (int id,
        JsonOwnedContext dbContext,
        CancellationToken cancellationToken) =>
    {
        var item = await dbContext.JsonOwners.FindAsync([id], cancellationToken: cancellationToken);
        if (item is null)
        {
            return Results.NotFound();
        }

        // Update some values
        item.Details.Name += $" updated_at_{DateTimeOffset.UtcNow}";
        item.Details.SubDetails[0].Value += $" updated_at_{DateTimeOffset.UtcNow}";

        // Log the change tracker state before saving
        Console.WriteLine(">>> " + dbContext.ChangeTracker.DebugView.ShortView);
        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            Console.WriteLine($">>> Entity: '{entry.Entity.GetType().Name}', State: {entry.State}");
            foreach (var prop in entry.Properties)
            {
                Console.WriteLine(
                    $">>>      '{prop.Metadata.Name}': FROM '{prop.OriginalValue}' -> '{prop.CurrentValue}'");
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(item);
    });

app.MapPost("/json-owner",
    async (JsonOwnedContext dbContext,
        CancellationToken cancellationToken) =>
    {
        var jsonOwner = new JsonOwner
        {
            Details = new JsonDetails
            {
                Name = "Owner 1",
                SubDetails =
                [
                    new JsonSubDetail { Value = "Owner 1 - SubDetail 1" },
                    new JsonSubDetail { Value = "Owner 1 - SubDetail 2" }
                ]
            }
        };

        dbContext.JsonOwners.Add(jsonOwner);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created($"/json-owner/{jsonOwner.Id}", jsonOwner);
    });


app.Run();