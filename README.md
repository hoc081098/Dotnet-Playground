# .NET Playground 🎮

A personal learning playground for exploring **C#**, **F#**, and **.NET** features, patterns, and best practices.

[![Build & Test 🧪](https://github.com/hoc081098/Dotnet-Playground/actions/workflows/build.yml/badge.svg)](https://github.com/hoc081098/Dotnet-Playground/actions/workflows/build.yml)
[![Hits](https://hits.sh/github.com/hoc081098/Dotnet-Playground.svg)](https://hits.sh/github.com/hoc081098/Dotnet-Playground/)

## 📁 Project Structure

```
├── CsharpPlayground/          # Main C# playground console app
│   ├── AsyncProgramming/      # Async/await, Task, CancellationToken
│   ├── Collections/           # Lists, Sets, Immutable collections
│   ├── DelegatesAndEvents/    # Delegates, Events, Lambdas
│   ├── LINQ/                  # LINQ, Expression Trees, Static Lambdas
│   ├── OOP/                   # Classes, Records, Interfaces, Generics
│   ├── Patterns/              # Pattern matching
│   └── ThreadPlayground/      # Threading, Atomic operations
│
├── CsharpPlayground.Tests/    # Unit tests (xUnit)
│
├── FsharpPlayground/          # F# playground - exploring FP concepts
│
└── WebAppPlayground/          # ASP.NET Core minimal API with EF Core
```

## 🛠️ Technologies

- **.NET 10** (Preview)
- **C# 14** / **F# 10**
- **ASP.NET Core** - Minimal APIs
- **Entity Framework Core** - PostgreSQL with JSON column support
- **xUnit** - Unit testing
- **SonarAnalyzer** - Code quality analysis

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/) (for PostgreSQL database)

### Run the projects

```bash
# Build all projects
dotnet build

# Run C# playground
dotnet run --project CsharpPlayground

# Run F# playground
dotnet run --project FsharpPlayground

# Run Web API (requires PostgreSQL)
docker compose up -d
dotnet run --project WebAppPlayground
```

### Run tests

```bash
dotnet test
```

## 📝 Topics Covered

### C# Features
- **Async/Await** - Task-based asynchronous programming, `IAsyncEnumerable`
- **LINQ** - Query syntax, method syntax, expression trees
- **Records** - Value semantics, immutability
- **Pattern Matching** - Switch expressions, type patterns
- **Nullable Reference Types** - `?`, `!`, null checks
- **Generics** - Constraints, covariance, contravariance

### F# Features
- **Async Computation Expressions** - `async { }`, `let!`, `Async.Parallel`
- **Pipe Operator** - `|>` for data flow
- **Pattern Matching** - Discriminated unions, active patterns

### ASP.NET Core
- **Minimal APIs** - Lightweight HTTP endpoints
- **EF Core** - JSON columns, migrations, PostgreSQL

## 📚 Documentation

- **[Kotlin/Java to C#/.NET Comparison Guide](Note/KOTLIN_TO_CSHARP.md)** - Comprehensive guide for Android/Kotlin developers learning C#/.NET

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**Petrus Nguyễn Thái Học** ([@hoc081098](https://github.com/hoc081098))

