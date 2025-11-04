namespace CsharpPlayground.LINQ;

/// <summary>
/// Demonstrates LINQ (Language Integrated Query) in C#.
/// LINQ is similar to:
/// - Kotlin's collection operations (map, filter, reduce, etc.)
/// - Java Streams API
/// - SQL queries
/// Key differences:
/// - Two syntax styles: Method syntax and Query syntax
/// - Deferred execution (lazy evaluation) by default
/// - IEnumerable&lt;T&gt; for in-memory queries
/// - IQueryable&lt;T&gt; for database queries (EF Core)
/// </summary>
public static class LinqPlayground
{
    private record Product(int Id, string Name, decimal Price, string Category, int Stock);
    private record Order(int Id, int ProductId, int Quantity, DateTime OrderDate);

    public static void Run()
    {
        Console.WriteLine("=== LINQ Basics ===");

        var products = new List<Product>
        {
            new(1, "Laptop", 1200m, "Electronics", 10),
            new(2, "Mouse", 25m, "Electronics", 50),
            new(3, "Keyboard", 75m, "Electronics", 30),
            new(4, "Chair", 300m, "Furniture", 15),
            new(5, "Desk", 500m, "Furniture", 8),
            new(6, "Monitor", 400m, "Electronics", 20),
            new(7, "Lamp", 45m, "Furniture", 25)
        };

        var orders = new List<Order>
        {
            new(1, 1, 2, DateTime.Now.AddDays(-5)),
            new(2, 2, 5, DateTime.Now.AddDays(-3)),
            new(3, 1, 1, DateTime.Now.AddDays(-2)),
            new(4, 4, 3, DateTime.Now.AddDays(-1))
        };

        // ===== Filtering (Where) - similar to Kotlin's filter =====
        var expensiveProducts = products.Where(p => p.Price > 100);
        Console.WriteLine("Expensive products (Price > 100):");
        foreach (var p in expensiveProducts)
        {
            Console.WriteLine($"  {p.Name}: ${p.Price}");
        }

        // ===== Projection (Select) - similar to Kotlin's map =====
        var productNames = products.Select(p => p.Name);
        Console.WriteLine($"\nProduct names: {string.Join(", ", productNames)}");

        // Anonymous type projection
        var productSummaries = products.Select(p => new { p.Name, p.Price, InStock = p.Stock > 0 });
        Console.WriteLine("\nProduct summaries:");
        foreach (var summary in productSummaries)
        {
            Console.WriteLine($"  {summary.Name}: ${summary.Price} (In Stock: {summary.InStock})");
        }

        // ===== Sorting =====
        var sortedByPrice = products.OrderBy(p => p.Price);
        Console.WriteLine("\nProducts sorted by price:");
        foreach (var p in sortedByPrice.Take(3))
        {
            Console.WriteLine($"  {p.Name}: ${p.Price}");
        }

        var sortedByPriceDesc = products.OrderByDescending(p => p.Price);
        Console.WriteLine("\nProducts sorted by price (descending):");
        foreach (var p in sortedByPriceDesc.Take(3))
        {
            Console.WriteLine($"  {p.Name}: ${p.Price}");
        }

        // Multiple sort keys
        var sortedMultiple = products.OrderBy(p => p.Category).ThenByDescending(p => p.Price);
        Console.WriteLine("\nProducts sorted by category, then price descending:");
        foreach (var p in sortedMultiple)
        {
            Console.WriteLine($"  {p.Category} - {p.Name}: ${p.Price}");
        }

        // ===== Grouping - similar to Kotlin's groupBy =====
        var groupedByCategory = products.GroupBy(p => p.Category);
        Console.WriteLine("\nProducts grouped by category:");
        foreach (var group in groupedByCategory)
        {
            Console.WriteLine($"  Category: {group.Key}");
            foreach (var p in group)
            {
                Console.WriteLine($"    - {p.Name}: ${p.Price}");
            }
        }

        // ===== Aggregation - similar to Kotlin's reduce, sum, etc. =====
        var totalValue = products.Sum(p => p.Price * p.Stock);
        var averagePrice = products.Average(p => p.Price);
        var maxPrice = products.Max(p => p.Price);
        var minPrice = products.Min(p => p.Price);
        var productCount = products.Count();
        var electronicsCount = products.Count(p => p.Category == "Electronics");

        Console.WriteLine($"\nAggregations:");
        Console.WriteLine($"  Total inventory value: ${totalValue}");
        Console.WriteLine($"  Average price: ${averagePrice:F2}");
        Console.WriteLine($"  Max price: ${maxPrice}");
        Console.WriteLine($"  Min price: ${minPrice}");
        Console.WriteLine($"  Total products: {productCount}");
        Console.WriteLine($"  Electronics count: {electronicsCount}");

        // ===== First, Last, Single =====
        var firstProduct = products.First();
        var firstElectronics = products.First(p => p.Category == "Electronics");
        var lastProduct = products.Last();
        var singleLaptop = products.Single(p => p.Name == "Laptop"); // Throws if not exactly one
        var firstOrDefaultCheap = products.FirstOrDefault(p => p.Price < 10); // Returns null if not found

        Console.WriteLine($"\nFirst product: {firstProduct.Name}");
        Console.WriteLine($"First electronics: {firstElectronics.Name}");
        Console.WriteLine($"Last product: {lastProduct.Name}");
        Console.WriteLine($"Single laptop: {singleLaptop.Name}");
        Console.WriteLine($"First cheap product: {firstOrDefaultCheap?.Name ?? "None"}");

        // ===== Any, All - similar to Kotlin's any, all =====
        var hasExpensiveProducts = products.Any(p => p.Price > 1000);
        var allInStock = products.All(p => p.Stock > 0);
        Console.WriteLine($"\nHas expensive products (>$1000): {hasExpensiveProducts}");
        Console.WriteLine($"All products in stock: {allInStock}");

        // ===== Skip, Take - pagination =====
        var page1 = products.Skip(0).Take(3);
        var page2 = products.Skip(3).Take(3);
        Console.WriteLine($"\nPage 1: {string.Join(", ", page1.Select(p => p.Name))}");
        Console.WriteLine($"Page 2: {string.Join(", ", page2.Select(p => p.Name))}");

        // ===== Distinct =====
        var categories = products.Select(p => p.Category).Distinct();
        Console.WriteLine($"\nDistinct categories: {string.Join(", ", categories)}");

        // ===== Join - similar to SQL JOIN =====
        var orderDetails = orders.Join(
            products,
            order => order.ProductId,
            product => product.Id,
            (order, product) => new
            {
                OrderId = order.Id,
                ProductName = product.Name,
                Quantity = order.Quantity,
                TotalPrice = product.Price * order.Quantity,
                OrderDate = order.OrderDate
            }
        );

        Console.WriteLine("\nOrder details (Join):");
        foreach (var detail in orderDetails)
        {
            Console.WriteLine($"  Order {detail.OrderId}: {detail.Quantity}x {detail.ProductName} = ${detail.TotalPrice} on {detail.OrderDate:yyyy-MM-dd}");
        }

        // ===== SelectMany - flatten nested collections =====
        var categoryWords = products.SelectMany(p => p.Category.Split(' '));
        Console.WriteLine($"\nAll category words: {string.Join(", ", categoryWords.Distinct())}");

        // ===== Query Syntax (alternative to method syntax) =====
        var queryResult = from p in products
                          where p.Price > 50 && p.Category == "Electronics"
                          orderby p.Price
                          select new { p.Name, p.Price };

        Console.WriteLine("\nQuery syntax result (Electronics > $50):");
        foreach (var item in queryResult)
        {
            Console.WriteLine($"  {item.Name}: ${item.Price}");
        }

        // ===== Complex query with multiple operations =====
        var categoryStats = products
            .GroupBy(p => p.Category)
            .Select(g => new
            {
                Category = g.Key,
                Count = g.Count(),
                TotalValue = g.Sum(p => p.Price * p.Stock),
                AvgPrice = g.Average(p => p.Price),
                TopProduct = g.OrderByDescending(p => p.Price).First().Name
            });

        Console.WriteLine("\nCategory statistics:");
        foreach (var stat in categoryStats)
        {
            Console.WriteLine($"  {stat.Category}:");
            Console.WriteLine($"    Count: {stat.Count}");
            Console.WriteLine($"    Total Value: ${stat.TotalValue}");
            Console.WriteLine($"    Avg Price: ${stat.AvgPrice:F2}");
            Console.WriteLine($"    Top Product: {stat.TopProduct}");
        }

        // ===== Deferred execution demonstration =====
        Console.WriteLine("\n=== Deferred Execution ===");
        var query = products.Where(p => p.Price > 100);
        Console.WriteLine("Query defined but not executed yet");

        products.Add(new Product(8, "Tablet", 600m, "Electronics", 12));
        Console.WriteLine("Added Tablet to products");

        // Query executes here and includes the newly added product
        Console.WriteLine($"Query executed - found {query.Count()} products");

        // ===== ToList(), ToArray() - immediate execution =====
        var productList = products.Where(p => p.Price > 100).ToList(); // Executes immediately
        products.Add(new Product(9, "Phone", 800m, "Electronics", 5));
        Console.WriteLine($"List count (doesn't include Phone): {productList.Count}");
        Console.WriteLine($"Query count (includes Phone): {products.Count(p => p.Price > 100)}");
    }
}
