using Microsoft.EntityFrameworkCore;
using WebAppPlayground.Data;

namespace WebAppPlayground;

public static class JsonOwnerEndpoints
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public void MapJsonOwnerEndpoints()
        {
            endpoints.MapGet("/json-owner/{id:int}",
                async (int id,
                    JsonOwnedContext dbContext,
                    CancellationToken cancellationToken) =>
                {
                    var item = await dbContext.JsonOwners
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                    return item is null ? Results.NotFound() : Results.Ok(item);
                });

            endpoints.MapPut("/json-owner/{id:int}",
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

            endpoints.MapPost("/json-owner",
                async (JsonOwnedContext dbContext,
                    CancellationToken cancellationToken) =>
                {
                    var currentUtc = DateTimeOffset.UtcNow;
                    var jsonOwner = new JsonOwner
                    {
                        Details = new JsonDetails
                        {
                            Name = "Owner " + currentUtc,
                            SubDetails =
                            [
                                new JsonSubDetail { Value = $"Owner {currentUtc} - SubDetail 1" },
                                new JsonSubDetail { Value = $"Owner {currentUtc} - SubDetail 2" }
                            ]
                        }
                    };

                    dbContext.JsonOwners.Add(jsonOwner);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    return Results.Created($"/json-owner/{jsonOwner.Id}", jsonOwner);
                });
        }
    }
}