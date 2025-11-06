using System.Security.AccessControl;
using CsharpPlayground.Collections;

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
            new(Id: 2, ProductId: 2, Quantity: 5, OrderDate: now.AddHours(value: -3)),
            new(Id: 3, ProductId: 1, Quantity: 1, OrderDate: now.AddHours(value: -2)),
            new(Id: 4, ProductId: 4, Quantity: 3, OrderDate: now.AddHours(value: 10)),
            new(Id: 5, ProductId: 100, Quantity: 5, OrderDate: now.AddHours(value: 10))
        ];
        PrintSeparator();

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
        var categoryStats = products
            .GroupBy(product => product.Category)
            .Select(grouping =>
            {
                // grouping is never empty here.
                return new
                {
                    Category = grouping.Key,
                    Amount = grouping.Count(),
                    TotalPrice = grouping.Sum(p => p.Price),
                    AveragePrice = grouping.Average(p => p.Price),
                    TopProduct = grouping.OrderByDescending(p => p.Price).First().Name,
                };
            });
        PrintResult("Products grouped by Category:", productsByCategories);
        PrintResult("Category Statistics:", categoryStats);
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

        // ===== Aggregation - similar to Kotlin's reduce, sum, etc. =====
        var totalValue = products.Sum(p => p.Price * p.Stock);
        // Use DefaultIfEmpty to avoid InvalidOperationException on empty collections for Average, Max, Min
        var averagePrice = products.DefaultIfEmpty().Average(p => p?.Price);
        var maxPrice = products.DefaultIfEmpty().Max(p => p?.Price);
        var minPrice = products.DefaultIfEmpty().Min(p => p?.Price);
        // It is more efficient to use Count property for collections, but using Count() here for demonstration
#pragma warning disable CA1829
        var productCount = products.Count();
#pragma warning restore CA1829
        var electronicsCount = products.Count(p => p.Category == "Electronics");
        Console.WriteLine("Aggregate Statistics:");
        Console.WriteLine($"- Total Inventory Value: {totalValue}");
        Console.WriteLine($"- Average Price: {averagePrice}");
        Console.WriteLine($"- Max Price: {maxPrice}");
        Console.WriteLine($"- Min Price: {minPrice}");
        Console.WriteLine($"- Total Product Count: {productCount}");
        Console.WriteLine($"- Electronics Product Count: {electronicsCount}");
        PrintSeparator();

        // ===== First, Last, Single - similar to Kotlin's first, last, single =====
        var firstOrder = orders.First();
        var firstOrderWithQuantityOne = orders.First(o => o.Quantity == 1);
        var lastOrder = orders.Last();
        var lastOrderForProduct2 = orders.Last(o => o.ProductId == 2);
        var firstOrDefaultCheap = products.FirstOrDefault(p => p.Price < 10); // Returns null if not found

        Console.WriteLine($"First Order: {firstOrder}");
        Console.WriteLine($"First Order with Quantity 1: {firstOrderWithQuantityOne}");
        Console.WriteLine($"Last Order: {lastOrder}");
        Console.WriteLine($"Last Order for ProductId 2: {lastOrderForProduct2}");
        Console.WriteLine($"FirstOrDefault for cheap product (< $10): {firstOrDefaultCheap?.ToString() ?? "null"}");
        try
        {
            var singleOrder = orders.Single();
            Console.WriteLine($"Single Order: {singleOrder}");
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(
                "Single() threw InvalidOperationException as expected because there are multiple orders.");
        }

        var single = new List<string> { "hello" }.Single();
        Console.WriteLine($"Single from single-item list: {single}");
        PrintSeparator();

        // ===== All, Any - similar to Kotlin's all, any =====
        var allInStock = products.All(p => p.Stock > 0m);
        var hasExpensiveProducts = products.Any(p => p.Price > 1000m);
        Console.WriteLine($"All Products In Stock: {allInStock}");
        Console.WriteLine($"Has Expensive Products: {hasExpensiveProducts}");
        PrintSeparator();

        // ===== Take, Skip - similar to Kotlin's take, drop =====
        var subProducts1 = products.Skip(2).Take(2).ToList();
        var subProducts2 = products.GetRange(index: 2, count: 2);
        var subProducts3 = products.GetRange(2..4);
        PrintResult($"Sub-Products In Stock 1", subProducts1);
        PrintResult($"Sub-Products In Stock 2", subProducts2);
        PrintResult($"Sub-Products In Stock 3", subProducts3);
        PrintSeparator();

        // ==== Distinct - similar to Kotlin's distinct =====
        var categories = products.Select(p => p.Category).Distinct();
        var dates = orders.Select(o => o.OrderDate.Date).Distinct();
        PrintResult("Distinct Product Categories: ", categories);
        PrintResult("Distinct Order Dates: ", dates);
        PrintSeparator();

        // ===== Join - similar to Kotlin's groupBy + flatMap =====
        IEnumerable<(Product p, Order o)> join1 = products.Join(inner: orders,
            outerKeySelector: p => p.Id,
            innerKeySelector: o => o.Id,
            resultSelector: (p, o) => (p, o));
        IEnumerable<(Product p, Order o)> join2 = from p in products
            join o in orders on p.Id equals o.Id
            select (p, o);
        PrintResult("Join Products and Orders on ProductId (method):", join1);
        PrintResult("Join Products and Orders on ProductId (query):", join2);
        PrintSeparator();

        var leftJoin1 = orders.GroupJoin(products,
                order => order.ProductId,
                product => product.Id,
                (order, productGroup) => (order, productGroup))
            .SelectMany(
                tuple => tuple.productGroup.DefaultIfEmpty(),
                (tuple, product) => new
                {
                    Order = tuple.order,
                    Product = product?.Name ?? "Product not found"
                }
            );
        var leftJoin2 = from o in orders
            join p in products on o.ProductId equals p.Id into productGroup
            from ps in productGroup.DefaultIfEmpty()
            select new
            {
                Order = o,
                Product = ps?.Name ?? "Product not found"
            };
        PrintResult("Left Join Orders and Products (method):", leftJoin1);
        PrintResult("Left Join Orders and Products (query):", leftJoin2);
        PrintSeparator();
    }

    private static void PrintSeparator() => Console.WriteLine(new string('-', 80));

    private static void PrintResult<T>(string title, IEnumerable<T> list)
    {
        Console.WriteLine(title);
        Console.WriteLine(string.Join(", \n", list));
    }
}