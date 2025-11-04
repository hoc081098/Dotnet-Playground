# C# and .NET Learning Guide for Android/Kotlin Developers

## Overview
This repository is a learning playground for a Senior Android Developer transitioning to C# and .NET backend development. It demonstrates key language features and backend concepts needed to reach mid-level .NET backend developer proficiency.

## Repository Structure

```
CsharpPlayground/
├── AsyncProgramming/      # Async/await patterns (similar to Kotlin coroutines)
├── Collections/           # Lists, Sets, Immutable collections
├── DelegatesAndEvents/    # Delegates, events, Func/Action types
├── LINQ/                  # Language Integrated Query (like Kotlin collections + SQL)
├── OOP/                   # Object-oriented programming concepts
├── Program.cs             # Main entry point demonstrating all features
└── Default.cs             # Default values demonstration
```

## Language Features Covered

### 1. **Async/Await Programming** (`AsyncProgramming/`)
**Kotlin/Java Equivalent:** Coroutines (Kotlin), CompletableFuture (Java)

- `async/await` keywords for asynchronous operations
- `Task` and `Task<T>` (similar to `Deferred<T>` in Kotlin)
- `Task.Run()` for CPU-bound work (like `withContext(Dispatchers.Default)`)
- `Task.WhenAll()` and `Task.WhenAny()` for parallel operations
- `CancellationToken` for cancellation support
- `ValueTask<T>` for performance-critical scenarios

**Key Differences from Kotlin:**
- C# uses `Task` instead of `Job`/`Deferred`
- `await` keyword required (not implicit like in Kotlin suspend functions)
- No concept of coroutine scope (CoroutineScope) in C#
- `ConfigureAwait(false)` for library code (no Kotlin equivalent)

### 2. **LINQ (Language Integrated Query)** (`LINQ/`)
**Kotlin/Java Equivalent:** Collection operations (Kotlin), Streams API (Java)

- **Filtering:** `Where()` → Kotlin's `filter()`
- **Projection:** `Select()` → Kotlin's `map()`
- **Sorting:** `OrderBy()`, `ThenBy()` → Kotlin's `sortedBy()`
- **Grouping:** `GroupBy()` → Kotlin's `groupBy()`
- **Aggregation:** `Sum()`, `Average()`, `Count()` → Kotlin's similar functions
- **Joins:** `Join()` → SQL-like joins
- **Query syntax:** SQL-like syntax alternative to method chaining

**Key Features:**
- Deferred execution (lazy evaluation) by default
- Two syntax styles: Method syntax and Query syntax
- `IEnumerable<T>` for in-memory queries
- `IQueryable<T>` for database queries (Entity Framework)

### 3. **Delegates and Events** (`DelegatesAndEvents/`)
**Kotlin/Java Equivalent:** Function types (Kotlin), Functional interfaces (Java)

- **Delegates:** Type-safe function pointers
- **Events:** Publisher-subscriber pattern
- `Func<T, TResult>` → Kotlin's `(T) -> Result`
- `Action<T>` → Kotlin's `(T) -> Unit`
- `Predicate<T>` → `(T) -> Boolean`

**Key Concepts:**
- Multicast delegates (multiple handlers)
- `event` keyword for encapsulation
- `EventHandler<TEventArgs>` pattern
- Lambda expressions and method references

### 4. **Collections** (`Collections/`)
**Comprehensive coverage of:**

- **Lists:**
  - `IReadOnlyList<T>` → Kotlin's `List<T>`
  - `List<T>` → Kotlin's `MutableList<T>` / `ArrayList<T>`
  - `LinkedList<T>` → Kotlin's `LinkedList<T>`
  - `ImmutableList<T>` → Immutable collections

- **Sets:**
  - `IReadOnlySet<T>` → Kotlin's `Set<T>`
  - `HashSet<T>` → Kotlin's `HashSet<T>`
  - `SortedSet<T>` → Kotlin's `TreeSet<T>`
  - `ImmutableHashSet<T>` → Immutable sets

**Key Differences:**
- `==` compares references, not contents (use `SequenceEqual()` or `SetEquals()`)
- Range access: `list[2..5]`, `list[^1]` (last element)

### 5. **Object-Oriented Programming** (`OOP/`)

#### **Records** (`RecordsPlayground.cs`)
**Kotlin Equivalent:** Data classes

- Value-based equality by default
- Immutable by default (`init` properties)
- `with` expression for copying with modifications → Kotlin's `copy()`
- Positional deconstruction
- Record structs (value types)

#### **Classes and Properties** (`Person.cs`, `Parent.cs`)
- Primary constructors (C# 12+)
- Auto-implemented properties
- `required` modifier for required properties
- `init`-only properties
- Computed properties (`=>` syntax)
- Access modifiers: `public`, `private`, `internal`, `protected`, `private protected`, `protected internal`

#### **Enums and Extensions** (`EnumExtensions.cs`)
- Enum utilities for finding by name or value
- Extension methods pattern
- `Enum.Parse<T>()`, `Enum.IsDefined()`

### 6. **Value Types and Default Values** (`Default.cs`)
Understanding default values for:
- Primitive types (`int`, `bool`, `char`, etc.)
- Reference types (null)
- Value types (struct, record struct)
- Nullable types (`int?`, `Nullable<T>`)

## Backend Concepts (To Be Added)

### Essential Mid-Level .NET Backend Skills (Not Yet Covered):

1. **ASP.NET Core Fundamentals**
   - Controllers and routing
   - Middleware pipeline
   - Dependency Injection (DI)
   - Configuration (appsettings.json, environment variables)
   
2. **Data Access**
   - Entity Framework Core
   - LINQ to Entities
   - Database migrations
   - Repository pattern
   
3. **Web APIs**
   - RESTful API design
   - HTTP client usage (`HttpClient`)
   - JSON serialization/deserialization (`System.Text.Json`)
   - Model validation
   
4. **Authentication & Authorization**
   - JWT tokens
   - OAuth 2.0
   - ASP.NET Core Identity
   - Policy-based authorization
   
5. **Testing**
   - xUnit (current test framework)
   - Moq for mocking
   - Integration tests
   - Test-driven development (TDD)
   
6. **Error Handling & Logging**
   - Exception handling patterns
   - Custom exceptions
   - ILogger interface
   - Structured logging (Serilog)
   
7. **Advanced Topics**
   - Middleware creation
   - Background services (`IHostedService`)
   - SignalR for real-time communication
   - gRPC services
   - Message queues (RabbitMQ, Azure Service Bus)

## Key Differences: Kotlin/Java vs C#/.NET

### Syntax Differences
| Concept | Kotlin/Java | C# |
|---------|-------------|-----|
| Nullable types | `String?` | `string?` |
| String interpolation | `"Hello $name"` | `$"Hello {name}"` |
| Collection literals | `listOf(1, 2, 3)` | `[1, 2, 3]` (C# 12+) |
| Lambda syntax | `{ x -> x * 2 }` | `x => x * 2` |
| Extension methods | `fun String.myExt()` | `static void MyExt(this string)` |
| Properties | `val name: String` | `string Name { get; }` |
| Async functions | `suspend fun` | `async Task` |

### Conceptual Differences
1. **Nullability:** Kotlin has null safety by default; C# requires enabling nullable reference types
2. **Coroutines vs Tasks:** Kotlin's coroutines are lighter; C# Tasks are thread-based
3. **Collections:** Kotlin distinguishes read-only (`List`) vs mutable (`MutableList`); C# uses `IReadOnlyList` and `List`
4. **Scope functions:** Kotlin has `let`, `apply`, `also`, `run`; C# doesn't have built-in equivalents
5. **Smart casts:** Kotlin has automatic smart casting; C# requires explicit casting after type checks

## Running the Examples

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run --project CsharpPlayground

# Run tests
dotnet test
```

## Learning Path Recommendations

### For Android/Kotlin Developers:
1. ✅ **Start here:** OOP concepts (already similar to Kotlin)
2. ✅ **Next:** LINQ (powerful query syntax)
3. ✅ **Then:** Async/await patterns (compare with coroutines)
4. ✅ **After:** Delegates and events (different from Kotlin)
5. 🔲 **Continue with:** ASP.NET Core basics
6. 🔲 **Master:** Entity Framework Core for database access
7. 🔲 **Advanced:** Middleware, authentication, and deployment

### Suggested Next Steps:
1. Build a simple REST API with ASP.NET Core
2. Add database access with Entity Framework Core
3. Implement authentication with JWT
4. Add unit and integration tests
5. Deploy to Azure or AWS

## Resources

### Official Documentation
- [Microsoft C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

### For Kotlin/Java Developers
- [C# for Java Developers](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/)
- [Kotlin to C# Comparison](https://kotlinlang.org/docs/comparison-to-java.html) (adapt concepts)

## Contributing
Feel free to add more examples and concepts as you learn!

## License
This is a learning playground - use it freely for educational purposes.
