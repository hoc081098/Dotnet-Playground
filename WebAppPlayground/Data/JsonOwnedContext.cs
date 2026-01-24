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
        modelBuilder.Entity<JsonOwner>().OwnsOne(x => x.Details, details =>
        {
            details.ToJson();
            details.OwnsMany(x => x.SubDetails);
        });
    }
}