using System.Linq.Expressions;

namespace CsharpPlayground.LINQ;

public static class ExpressionTreePlayground
{
    public static void Run()
    {
        PrintSeparator();
        Console.WriteLine("=== Expression Tree Playground ===");

        // 1️⃣. Create expression tree explicitly
        Expression<Func<int, bool>> isEvenExpr = x => x % 2 == 0;

        Console.WriteLine($"Expression: {isEvenExpr}");
        Console.WriteLine($"Body: {isEvenExpr.Body}");
        Console.WriteLine($"Parameters: {string.Join(", ", isEvenExpr.Parameters.Select(p => p.Name))}");
        PrintSeparator();

        // 2️⃣. Compile expression tree to executable delegate
        Func<int, bool> isEvenFunc = isEvenExpr.Compile();
        Console.WriteLine($"isEvenFunc(4) = {isEvenFunc(4)}");
        Console.WriteLine($"isEvenFunc(5) = {isEvenFunc(5)}");
        PrintSeparator();

        // 3️⃣. Build expression tree manually (x => x > 10)
        ParameterExpression xParam = Expression.Parameter(typeof(int), "x");
        ConstantExpression const10 = Expression.Constant(10);
        BinaryExpression body = Expression.GreaterThan(xParam, const10);
        Expression<Func<int, bool>> greaterThanTenExpr = Expression.Lambda<Func<int, bool>>(body, xParam);

        Console.WriteLine($"Manually built: {greaterThanTenExpr}");
        Func<int, bool> greaterThanTenFunc = greaterThanTenExpr.Compile();
        Console.WriteLine($"greaterThanTenFunc(5) = {greaterThanTenFunc(5)}");
        Console.WriteLine($"greaterThanTenFunc(15) = {greaterThanTenFunc(15)}");
        PrintSeparator();

        // 4️⃣. Modify expression tree dynamically
        // Replace constant "10" -> "20"
        var modified = new ReplaceConstantVisitor(10, 20).Visit(greaterThanTenExpr);
        var newLambda = (Expression<Func<int, bool>>)modified;
        Func<int, bool> greaterThanTwenty = newLambda.Compile();
        Console.WriteLine($"Modified expression: {newLambda}");
        Console.WriteLine($"greaterThanTwenty(15) = {greaterThanTwenty(15)}");
        Console.WriteLine($"greaterThanTwenty(25) = {greaterThanTwenty(25)}");
        PrintSeparator();

        // 5️⃣. Show how LINQ providers use Expression Trees
        IQueryable<int> numbers = new[] { 5, 10, 15, 20, 25 }.AsQueryable();
        var query = numbers.Where(isEvenExpr);
        Console.WriteLine($"Query Expression Tree: {query.Expression}");
        Console.WriteLine($"Query Provider Type: {query.Provider.GetType().Name}");
        PrintSeparator();

        Console.WriteLine("✅ Expression Tree Playground finished successfully.");
    }

    private static void PrintSeparator() => Console.WriteLine(new string('-', 80));

    // Custom visitor that replaces a specific constant in an expression tree
    private sealed class ReplaceConstantVisitor(object oldValue, object newValue) : ExpressionVisitor
    {
        protected override Expression VisitConstant(ConstantExpression node)
        {
            return node.Value == oldValue
                ? Expression.Constant(newValue, node.Type)
                : base.VisitConstant(node);
        }
    }
}