namespace CsharpPlayground.OOP;

/// <summary>
/// Provides utility extensions for working with enum types.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Finds the first enum value that matches the specified predicate.
    /// Throws an <see cref="InvalidOperationException"/> if no match is found.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="predicate">A function used to test each enum value.</param>
    /// <returns>The first matching enum value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no enum value matches the predicate.</exception>
    /// <example>
    /// <code>
    /// var result = EnumExtensions.FindOrThrow&lt;DayOfWeek&gt;(d => d.ToString() == "Monday");
    /// </code>
    /// </example>
    public static T FindOrThrow<T>(Func<T, bool> predicate)
        where T : struct, Enum
        => Enum.GetValues<T>().First(predicate);

    /// <summary>
    /// Finds the first enum value that matches the specified predicate,
    /// or returns the default value if no match is found.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="predicate">A function used to test each enum value.</param>
    /// <returns>The first matching enum value, or the default value if not found.</returns>
    /// <example>
    /// <code>
    /// var result = EnumExtensions.FindOrDefault&lt;DayOfWeek&gt;(d => d == DayOfWeek.Sunday);
    /// </code>
    /// </example>
    public static T FindOrDefault<T>(Func<T, bool> predicate)
        where T : struct, Enum
        => Enum.GetValues<T>().FirstOrDefault(predicate);

    /// <summary>
    /// Finds an enum value by its name.
    /// Throws an <see cref="ArgumentException"/> if no match is found.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="name">The name of the enum member to find.</param>
    /// <param name="ignoreCase">A boolean indicating whether to ignore case during the search.</param>
    /// <returns>The enum value that has the specified name.</returns>
    /// <exception cref="ArgumentException">Thrown when no enum member matches the given name.</exception>
    /// <example>
    /// <code>
    /// var day = EnumExtensions.FindByNameOrThrow&lt;DayOfWeek&gt;("Monday");
    /// </code>
    /// </example>
    public static T FindByNameOrThrow<T>(string name, bool ignoreCase = false)
        where T : struct, Enum
        => Enum.Parse<T>(name, ignoreCase);

    /// <summary>
    /// Finds an enum value by its underlying integer value.
    /// Throws an <see cref="ArgumentException"/> if no match is found.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="value">The integer value of the enum member to find.</param>
    /// <returns>The enum value that has the specified underlying integer value.</returns>
    /// <exception cref="ArgumentException">Thrown when no enum member matches the given value.</exception>
    /// <example>
    /// <code>
    /// var color = EnumExtensions.FindByValueOrThrow&lt;ConsoleColor&gt;(12);
    /// </code>
    /// </example>
    public static T FindByValueOrThrow<T>(int value)
        where T : struct, Enum =>
        Enum.IsDefined(typeof(T), value)
            ? (T)Enum.ToObject(typeof(T), value)
            : throw new ArgumentException($"No enum value of type {typeof(T).Name} with value {value} found.");
}