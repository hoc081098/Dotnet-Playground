# Kotlin/Java to C#/.NET Comparison Guide

## Quick Reference for Android Developers Learning C#/.NET

This guide helps Android/Kotlin developers quickly map their existing knowledge to C#/.NET equivalents.

---

## Table of Contents

### Part I: Language Basics
1. [I. Basic Syntax](#i-basic-syntax)
2. [II. Collections](#ii-collections)
3. [III. Functions and Lambdas](#iii-functions-and-lambdas)
4. [IV. Classes and Objects](#iv-classes-and-objects)
5. [V. Null Safety](#v-null-safety)

### Part II: Advanced Features
6. [VI. Async Programming](#vi-async-programming)
7. [VII. Properties and Fields](#vii-properties-and-fields)
8. [VIII. Interfaces and Inheritance](#viii-interfaces-and-inheritance)
9. [IX. Generics](#ix-generics)
10. [X. Common Patterns](#x-common-patterns)

### Part III: Summary & Resources
11. [XI. Key Differences Summary](#xi-key-differences-summary)
12. [XII. Recommended Learning Path](#xii-recommended-learning-path)
13. [XIII. Additional Resources](#xiii-additional-resources)

### Part IV: Repository Coverage
14. [XIV. Language Features Covered](#xiv-language-features-covered)
15. [XV. Backend Concepts (To Be Added)](#xv-backend-concepts-to-be-added)
16. [XVI. Key Differences: Kotlin/Java vs C#/.NET](#xvi-key-differences-kotlinjava-vs-cnet)

---

## I. Basic Syntax

### Variables and Constants

| Kotlin                | C#                                   | Notes                                       |
|-----------------------|--------------------------------------|---------------------------------------------|
| `val x = 10`          | `var x = 10;` or `const int x = 10;` | C# `var` is type inference, not immutable   |
| `var y = 20`          | `int y = 20;`                        | C# doesn't distinguish mutable with keyword |
| `const val MAX = 100` | `const int MAX = 100;`               | Compile-time constants                      |
| `val name: String`    | `string name`                        | Explicit type declaration                   |

### String Operations

| Kotlin                    | C#                                                    | Notes                    |
|---------------------------|-------------------------------------------------------|--------------------------|
| `"Hello $name"`           | `$"Hello {name}"`                                     | String interpolation     |
| `"Score: ${score * 2}"`   | `$"Score: {score * 2}"`                               | Expression interpolation |
| `"""Multi-line string"""` | `@"Multi-line string"` or `"""Multi-line"""` (C# 11+) | Multi-line strings       |
| `"kotlin".repeat(3)`      | `new string('-', 3)`                                  | String repetition        |

### Control Flow

| Kotlin                      | C#                                         | Notes                |
|-----------------------------|--------------------------------------------|----------------------|
| `if (x > 0) y else z`       | `x > 0 ? y : z`                            | Ternary operator     |
| `when (x) { ... }`          | `switch (x) { ... }` or `x switch { ... }` | Pattern matching     |
| `for (i in 0..10)`          | `for (int i = 0; i <= 10; i++)`            | Range loops          |
| `listOf(1,2,3).forEach { }` | `new[] {1,2,3}.ForEach(x => {})`           | Collection iteration |

---

## II. Collections

### List Operations

| Kotlin                      | C#                                                | Notes              |
|-----------------------------|---------------------------------------------------|--------------------|
| `listOf(1, 2, 3)`           | `new List<int> {1, 2, 3}` or `[1, 2, 3]` (C# 12+) | List creation      |
| `List<Int>` (immutable)     | `IReadOnlyList<int>`                              | Read-only list     |
| `MutableList<Int>`          | `List<int>`                                       | Mutable list       |
| `list.map { it * 2 }`       | `list.Select(x => x * 2)`                         | Map/Select         |
| `list.filter { it > 5 }`    | `list.Where(x => x > 5)`                          | Filter/Where       |
| `list.groupBy { it.age }`   | `list.GroupBy(x => x.Age)`                        | GroupBy            |
| `list.sortedBy { it.name }` | `list.OrderBy(x => x.Name)`                       | Sorting            |
| `list.firstOrNull()`        | `list.FirstOrDefault()`                           | Safe first element |
| `list.any { it > 10 }`      | `list.Any(x => x > 10)`                           | Any predicate      |
| `list.all { it > 0 }`       | `list.All(x => x > 0)`                            | All predicate      |

### Set Operations

| Kotlin            | C#                            | Notes         |
|-------------------|-------------------------------|---------------|
| `setOf(1, 2, 3)`  | `new HashSet<int> {1, 2, 3}`  | Set creation  |
| `Set<Int>`        | `IReadOnlySet<int>`           | Read-only set |
| `MutableSet<Int>` | `HashSet<int>` or `ISet<int>` | Mutable set   |
| `TreeSet<Int>`    | `SortedSet<int>`              | Sorted set    |

### Map Operations

| Kotlin                    | C#                                          | Notes                            |
|---------------------------|---------------------------------------------|----------------------------------|
| `mapOf("a" to 1)`         | `new Dictionary<string, int> { ["a"] = 1 }` | Map creation                     |
| `Map<String, Int>`        | `IReadOnlyDictionary<string, int>`          | Read-only map                    |
| `MutableMap<String, Int>` | `Dictionary<string, int>`                   | Mutable map                      |
| `map["key"]`              | `map["key"]`                                | Key access (throws if not found) |
| `map.get("key")`          | `map.TryGetValue("key", out var value)`     | Safe get                         |

---

## III. Functions and Lambdas

### Function Declarations

| Kotlin                                          | C#                                        | Notes                |
|-------------------------------------------------|-------------------------------------------|----------------------|
| `fun add(a: Int, b: Int): Int { return a + b }` | `int Add(int a, int b) { return a + b; }` | Function declaration |
| `fun add(a: Int, b: Int) = a + b`               | `int Add(int a, int b) => a + b;`         | Expression body      |
| `fun log(msg: String = "default")`              | `void Log(string msg = "default")`        | Default parameters   |

### Lambda Expressions

| Kotlin                         | C#                                        | Notes                     |
|--------------------------------|-------------------------------------------|---------------------------|
| `{ x -> x * 2 }`               | `x => x * 2`                              | Lambda syntax             |
| `{ x, y -> x + y }`            | `(x, y) => x + y`                         | Multi-parameter lambda    |
| `list.forEach { println(it) }` | `list.ForEach(x => Console.WriteLine(x))` | Lambda with one parameter |

### Function Types

| Kotlin                  | C#                                    | Notes                         |
|-------------------------|---------------------------------------|-------------------------------|
| `(Int) -> String`       | `Func<int, string>`                   | Function with return value    |
| `(Int, String) -> Unit` | `Action<int, string>`                 | Function without return value |
| `(Int) -> Boolean`      | `Predicate<int>` or `Func<int, bool>` | Predicate function            |

### Higher-Order Functions

| Kotlin                                  | C#                                     | Notes                |
|-----------------------------------------|----------------------------------------|----------------------|
| `fun operate(op: (Int, Int) -> Int)`    | `void Operate(Func<int, int, int> op)` | Function parameter   |
| `fun getOperation(): (Int, Int) -> Int` | `Func<int, int, int> GetOperation()`   | Function return type |

---

## IV. Classes and Objects

### Class Declaration

| Kotlin                                           | C#                                                                       | Notes               |
|--------------------------------------------------|--------------------------------------------------------------------------|---------------------|
| `class Person(val name: String)`                 | `class Person(string name) { public string Name { get; } = name; }`      | Primary constructor |
| `class Person { }`                               | `class Person { }`                                                       | Empty class         |
| `data class User(val id: Int, val name: String)` | `record User(int Id, string Name);`                                      | Data class/Record   |
| `object Singleton`                               | `public class Singleton { private static Singleton _instance = new(); }` | Singleton           |

### Properties

| Kotlin                                | C#                                      | Notes               |
|---------------------------------------|-----------------------------------------|---------------------|
| `val name: String`                    | `string Name { get; }`                  | Read-only property  |
| `var age: Int`                        | `int Age { get; set; }`                 | Mutable property    |
| `val fullName get() = "$first $last"` | `string FullName => $"{First} {Last}";` | Computed property   |
| `lateinit var text: String`           | `string text = null!;`                  | Late initialization |

### Constructors

| Kotlin                                      | C#                                           | Notes                        |
|---------------------------------------------|----------------------------------------------|------------------------------|
| `class Person(name: String)`                | `class Person(string name)`                  | Primary constructor (C# 12+) |
| `constructor(name: String) : this(name, 0)` | `public Person(string name) : this(name, 0)` | Secondary constructor        |
| `init { }`                                  | Constructor body in primary constructor      | Initialization block         |

---

## V. Null Safety

### Nullable Types

| Kotlin                       | C#                                   | Notes                            |
|------------------------------|--------------------------------------|----------------------------------|
| `String?`                    | `string?`                            | Nullable reference type          |
| `Int?`                       | `int?`                               | Nullable value type              |
| `val len = str?.length`      | `var len = str?.Length;`             | Safe call operator               |
| `val len = str?.length ?: 0` | `var len = str?.Length ?? 0;`        | Elvis operator / Null coalescing |
| `str!!`                      | No equivalent (use `!` with caution) | Force non-null assertion         |
| `str?.let { }`               | `if (str != null) { }`               | Safe call with lambda            |

### Null Checks

| Kotlin                          | C#                                     | Notes          |
|---------------------------------|----------------------------------------|----------------|
| `if (x != null) { x.method() }` | `if (x != null) { x.Method(); }`       | Null check     |
| Smart cast after null check     | Pattern matching: `if (x is not null)` | Type narrowing |

---

## VI. Async Programming

### Async/Await

| Kotlin                            | C#                                | Notes           |
|-----------------------------------|-----------------------------------|-----------------|
| `suspend fun fetchData(): String` | `async Task<string> FetchData()`  | Async function  |
| `val result = fetchData()`        | `var result = await FetchData();` | Await call      |
| `launch { }`                      | `Task.Run(() => { })`             | Background task |
| `async { }`                       | `Task.Run(() => { })`             | Async block     |
| `withContext(Dispatchers.IO) { }` | `await Task.Run(() => { })`       | Switch context  |
| `delay(1000)`                     | `await Task.Delay(1000);`         | Delay           |
| `Job`                             | `Task`                            | Async job/task  |
| `Deferred<T>`                     | `Task<T>`                         | Async result    |

### Parallel Execution

| Kotlin                                     | C#                                  | Notes              |
|--------------------------------------------|-------------------------------------|--------------------|
| `coroutineScope { launch { } launch { } }` | `await Task.WhenAll(task1, task2);` | Parallel execution |
| `select { }`                               | `await Task.WhenAny(task1, task2);` | First completed    |

---

## VII. Properties and Fields

### Access Modifiers

| Kotlin        | C#                   | Notes                  |
|---------------|----------------------|------------------------|
| `public`      | `public`             | Public access          |
| `private`     | `private`            | Private access         |
| `protected`   | `protected`          | Protected access       |
| `internal`    | `internal`           | Assembly/module access |
| No equivalent | `protected internal` | Protected OR internal  |
| No equivalent | `private protected`  | Protected AND private  |

### Static Members

| Kotlin                              | C#                     | Notes         |
|-------------------------------------|------------------------|---------------|
| `companion object { val x = 10 }`   | `static int x = 10;`   | Static field  |
| `companion object { fun method() }` | `static void Method()` | Static method |

---

## VIII. Interfaces and Inheritance

### Interface Declaration

| Kotlin                                                   | C#                                                       | Notes                  |
|----------------------------------------------------------|----------------------------------------------------------|------------------------|
| `interface Drawable { fun draw() }`                      | `interface IDrawable { void Draw(); }`                   | Interface              |
| `interface Named { val name: String get() = "default" }` | `interface INamed { string Name => "default"; }` (C# 8+) | Default implementation |

### Inheritance

| Kotlin                           | C#                              | Notes               |
|----------------------------------|---------------------------------|---------------------|
| `class Dog : Animal()`           | `class Dog : Animal`            | Inheritance         |
| `class Dog : Animal(), Runnable` | `class Dog : Animal, IRunnable` | Multiple interfaces |
| `override fun method()`          | `override void Method()`        | Override method     |
| `open class Animal`              | `class Animal` (not sealed)     | Inheritable class   |
| `abstract class Shape`           | `abstract class Shape`          | Abstract class      |

---

## IX. Generics

### Generic Types

| Kotlin                          | C#                                         | Notes                     |
|---------------------------------|--------------------------------------------|---------------------------|
| `class Box<T>(val value: T)`    | `class Box<T> { public T Value { get; } }` | Generic class             |
| `fun <T> identity(value: T): T` | `T Identity<T>(T value)`                   | Generic method            |
| `where T : Comparable<T>`       | `where T : IComparable<T>`                 | Generic constraint        |
| `T : Any`                       | `where T : class`                          | Reference type constraint |
| `out T`                         | `out T`                                    | Covariance                |
| `in T`                          | `in T`                                     | Contravariance            |

---

## X. Common Patterns

### Extension Functions

| Kotlin                                    | C#                                             | Notes                |
|-------------------------------------------|------------------------------------------------|----------------------|
| `fun String.reversed() = this.reversed()` | `static string Reversed(this string s) => ...` | Extension method     |
| Must be in object/companion               | Must be in static class                        | Location requirement |

### Sealed Classes

| Kotlin                | C#                                                 | Notes            |
|-----------------------|----------------------------------------------------|------------------|
| `sealed class Result` | `abstract class Result` (C# 9: record hierarchies) | Sealed hierarchy |
| `sealed interface`    | No direct equivalent                               | Sealed interface |

### Scope Functions

| Kotlin                      | C#                                         | Notes          |
|-----------------------------|--------------------------------------------|----------------|
| `obj.let { it.method() }`   | No built-in equivalent, use local variable | Let function   |
| `obj.apply { this.x = 5 }`  | Object initializer: `new Obj { X = 5 }`    | Apply function |
| `obj.also { println(it) }`  | No built-in equivalent                     | Also function  |
| `obj.run { this.method() }` | No built-in equivalent                     | Run function   |

### Delegates

| Kotlin             | C#                           | Notes               |
|--------------------|------------------------------|---------------------|
| `by lazy { }`      | `Lazy<T>`                    | Lazy initialization |
| `by remember { }`  | No built-in equivalent       | Property delegation |
| Property delegates | Custom via get/set accessors | Delegation pattern  |

---

## XI. Key Differences Summary

### Major Conceptual Differences

1. **Nullability**: Kotlin is null-safe by default; C# requires enabling nullable reference types (`#nullable enable`)

2. **Coroutines vs Tasks**:
    - Kotlin coroutines are lightweight and suspend-based
    - C# Tasks are thread-based and use async/await

3. **Collections**:
    - Kotlin distinguishes `List` (read-only) vs `MutableList`
    - C# uses `IReadOnlyList` vs `List` (more explicit)

4. **Properties**:
    - Kotlin properties are first-class language features
    - C# properties are syntactic sugar over get/set methods

5. **Extension Functions**:
    - Kotlin: defined anywhere
    - C#: must be in static class

6. **Smart Casts**:
    - Kotlin: automatic after type check
    - C#: pattern matching with explicit cast

7. **Scope Functions**:
    - Kotlin: built-in `let`, `apply`, `run`, `also`, `with`
    - C#: no built-in equivalents

### Ecosystem Differences

| Kotlin/Android  | C#/.NET                            | Purpose              |
|-----------------|------------------------------------|----------------------|
| Gradle          | MSBuild / .NET CLI                 | Build system         |
| JUnit           | xUnit / NUnit / MSTest             | Testing framework    |
| Mockito / MockK | Moq / NSubstitute                  | Mocking framework    |
| Retrofit        | HttpClient / Refit                 | HTTP client          |
| Room            | Entity Framework Core              | ORM                  |
| Gson / Moshi    | System.Text.Json / Newtonsoft.Json | JSON serialization   |
| Coroutines      | Task Parallel Library (TPL)        | Async programming    |
| Jetpack Compose | Blazor / MAUI                      | UI framework         |
| Dagger / Hilt   | Built-in DI container              | Dependency injection |

---

## XII. Recommended Learning Path

1. ✅ **Master basic syntax differences** (strings, collections, null safety)
2. ✅ **Understand async/await** (compared to Kotlin coroutines)
3. ✅ **Learn LINQ** (powerful query capabilities)
4. ✅ **Practice with delegates and events** (different from Kotlin lambdas)
5. 🔲 **Build REST APIs with ASP.NET Core**
6. 🔲 **Learn Entity Framework Core** (ORM for database access)
7. 🔲 **Implement authentication** (JWT, OAuth)
8. 🔲 **Master dependency injection** (built into ASP.NET Core)
9. 🔲 **Deploy to cloud** (Azure, AWS, or other providers)

---

## XIII. Additional Resources

- [Microsoft C# Guide](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [Kotlin to C# Playground](https://github.com/hoc081098/Dotnet-Playground) (this repository)
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

---

## XIV. Language Features Covered

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

## XV. Backend Concepts (To Be Added)

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

## XVI. Key Differences: Kotlin/Java vs C#/.NET

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
