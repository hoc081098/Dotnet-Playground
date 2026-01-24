using Microsoft.EntityFrameworkCore;

namespace WebAppPlayground.Data;

public class JsonOwner
{
    public int Id { get; set; }
    public required JsonDetails Details { get; set; }
}

public class JsonDetails
{
    public string? Name { get; set; }
    public required List<JsonSubDetail> SubDetails { get; set; }
}

public class JsonSubDetail
{
    public string? Value { get; set; }
}

public class JsonOwnedContext(DbContextOptions<JsonOwnedContext> options) : DbContext(options)
{
    public DbSet<JsonOwner> JsonOwners => Set<JsonOwner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // JSON structure in DB for `JsonOwner.Details`:
        // {
        //     "Name": "Owner 1 updated_at_24/1/2026 13:21:59 +00:00 updated_at_24/1/2026 13:23:46 +00:00",
        //     "SubDetails": [
        //     {
        //         "Value": "Owner 1 - SubDetail 1 updated_at_24/1/2026 13:21:59 +00:00 updated_at_24/1/2026 13:23:46 +00:00"
        //     },
        //     {
        //         "Value": "Owner 1 - SubDetail 2"
        //     }
        //     ]
        // }
        modelBuilder.Entity<JsonOwner>().OwnsOne(x => x.Details, details =>
        {
            // ToJson: This method should only be specified for the outer-most owned entity in the given ownership structure.
            // All entities owned by this will be automatically mapped to the same JSON column.
            // The ownerships must still be explicitly defined.
            // Name of the navigation will be used as the JSON column name.
            details.ToJson();
            details.OwnsMany(x => x.SubDetails);
        });
    }
}