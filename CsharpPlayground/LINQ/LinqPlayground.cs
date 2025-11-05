namespace CsharpPlayground.LINQ;

/// <summary>
/// Demonstrates LINQ (Language Integrated Query) in C#.
/// 
/// LINQ is similar to:
///  - Kotlin's collection operations (map, filter, reduce, etc.)
///  - Java Streams API
///  - SQL queries
///
/// Key differences:
///  - Two syntax styles: Method syntax and Query syntax
///  - Deferred execution (lazy evaluation) by default
///  - IEnumerable&lt;T&gt; for in-memory queries
///  - IQueryable&lt;T&gt; for database queries (EF Core)
/// </summary>
public static class LinqPlayground
{
    private record Product(int Id, string Name, decimal Price, string Category, int Stock);

    private record Order(int Id, int ProductId, int Quantity, DateTime OrderDate);

    public static void Run()
    {
        List<Product> products =
        [
            new(Id: 1, Name: "Laptop", Price: 1200m, Category: "Electronics", Stock: 10),
            new(Id: 2, Name: "Mouse", Price: 25m, Category: "Electronics", Stock: 50),
            new(Id: 3, Name: "Keyboard", Price: 75m, Category: "Electronics", Stock: 30),
            new(Id: 4, Name: "Chair", Price: 300m, Category: "Furniture", Stock: 15),
            new(Id: 5, Name: "Desk", Price: 500m, Category: "Furniture", Stock: 8),
            new(Id: 6, Name: "Monitor", Price: 400m, Category: "Electronics", Stock: 20),
            new(Id: 7, Name: "Lamp", Price: 45m, Category: "Furniture", Stock: 25),
            new(Id: 8, Name: "Phone", Price: 300m, Category: "Electronics", Stock: 25)
        ];
        var now = DateTime.Now;
        List<Order> orders =
        [
            new(Id: 1, ProductId: 1, Quantity: 2, OrderDate: now.AddDays(value: -5)),
            new(Id: 2, ProductId: 2, Quantity: 5, OrderDate: now.AddDays(value: -3)),
            new(Id: 3, ProductId: 1, Quantity: 1, OrderDate: now.AddDays(value: -2)),
            new(Id: 4, ProductId: 4, Quantity: 3, OrderDate: now.AddDays(value: -1))
        ];

        // ===== Filtering (Where) - similar to Kotlin's filter =====
        var filteredProducts1 = products.Where(p => p is { Price: > 100, Stock: > 10 });
        var filteredProducts2 = from p in products
            where p is { Price: > 100, Stock: > 10 }
            select p;
        PrintResult("Filtered Products (Price > 100 and Stock > 10)", filteredProducts1);
        PrintResult("Filtered Products (Price > 100 and Stock > 10)", filteredProducts2);
        PrintSeparator();

        // ===== Projection (Select) - similar to Kotlin's map =====
        var nameAndPriceTuples1 = products.Select(p => new { p.Name, p.Price });
        var nameAndPriceTuples2 = from p in products
            select new { p.Name, p.Price };
        PrintResult("Select Product Names and Prices", nameAndPriceTuples1);
        PrintResult("Select Product Names and Prices", nameAndPriceTuples2);
        PrintSeparator();

        // ===== FlatMapping (SelectMany) - similar to Kotlin's flatMap =====
        var names1 = products.SelectMany(p => Enumerable.Repeat(p.Name, 2));
        var names2 = from p in products
            from n in Enumerable.Repeat(p.Name, 2)
            select n;
        Console.WriteLine("FlatMap Product Names (each repeated twice): " + string.Join(", ", names1));
        Console.WriteLine("FlatMap Product Names (each repeated twice): " + string.Join(", ", names2));
        PrintSeparator();

        // ===== Reduction (Aggregate) - similar to Kotlin's reduce/fold =====
        var totalPrices = products.Aggregate(0m, (acc, p) => acc + p.Price);
        Console.WriteLine($"Total Prices of all Products: {totalPrices}");

        // ===== Aggregation (AggregateBy + ToDictionary) - similar to Kotlin's groupingBy { ... }.fold() =====
        Dictionary<string, string> productByCategories = products.AggregateBy(
            keySelector: p => p.Category,
            seed: "",
            func: (acc, e) => string.IsNullOrEmpty(acc) ? e.Name : $"{acc}, {e.Name}"
        ).ToDictionary();
        PrintResult("Product names aggregated by Category:", productByCategories);
        PrintSeparator();

        // ===== Grouping (GroupBy + ToDictionary) - similar to Kotlin's groupBy =====
        Dictionary<string, List<Product>> productsByCategories = products
            .GroupBy(
                keySelector: p => p.Category,
                elementSelector: p => p
            )
            .ToDictionary(
                keySelector: g => g.Key,
                elementSelector: g => g.ToList()
            );
        PrintResult("Products grouped by Category:", productsByCategories);
        PrintSeparator();


        // ==== Sorting (OrderBy, ThenBy) - similar to Kotlin's sortedBy, sortedWith =====
        var top5ProductsByPriceDesc1 = products.OrderByDescending(p => p.Price)
            .ThenBy(p => p.Name)
            .Take(5);
        var top5ProductsByPriceDesc2 = (
            from p in products
            orderby p.Price descending, p.Name
            select p
        ).Take(5);

        PrintResult("Products sorted by Price (desc) then Name (asc):", top5ProductsByPriceDesc1);
        PrintResult("Products sorted by Price (desc) then Name (asc):", top5ProductsByPriceDesc2);
        PrintSeparator();
    }

    private static void PrintSeparator() => Console.WriteLine(new string('-', 80));

    private static void PrintResult<T>(string title, IEnumerable<T> list)
    {
        Console.WriteLine(title);
        Console.WriteLine(string.Join(", \n", list));
    }
}