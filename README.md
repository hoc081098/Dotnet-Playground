### 🧠 C# Naming Convention Cheat Sheet

- `camelCase`: local variables, function parameters
- `_camelCase`: private/internal mutable fields
- `PascalCase`: everything else (public, const, static readonly, property, method, type, enum, interface)

| Style        | Used for                                                                              | Examples                                                      |
|:-------------|:--------------------------------------------------------------------------------------|:--------------------------------------------------------------|
| `camelCase`  | Local variables, parameters                                                           | `count`, `userName`                                           |
| `_camelCase` | Private/internal mutable fields                                                       | `_count`, `_cache`                                            |
| `PascalCase` | Public members, const, static readonly, properties, methods, types, enums, interfaces | `FullName`, `MaxValue`, `ToString()`, `Person`, `IRepository` |