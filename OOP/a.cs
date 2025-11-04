namespace CsharpPlayground.OOP;

#pragma warning disable CS0169 // Field is never used
#pragma warning disable CS0414 // Field is assigned but its value is never used
internal class test
{
    private int myValue; // ❌ error (_camelCase)
    private static readonly int readOnlyValue = 42; // ❌ error (PascalCase)

    public const int ConstantValue = 5; // ✅ ok
    private static readonly IReadOnlySet<string> Roles = new HashSet<string>() { "Admin", "User" }; // ✅ ok

    private int _validValue; // ✅ ok
    private static int _staticValue; // ✅ ok

    public static readonly int Default = 0; // ✅ ok
}
#pragma warning restore CS0414 // Field is assigned but its value is never used
#pragma warning restore CS0169 // Field is never used