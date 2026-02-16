[//]: # (Rules for code writing:)

[//]: # (1. Use records if it's valid instead of classes)

[//]: # (2. If we have option to mark property as required do it, instead of [required] attributes)

[//]: # (3. You should always create interfaces and we should follow pattern: SomethingNew : ISomethingNew)

[//]: # (4. Use primary ctor if possible)

[//]: # (5. Merge into patterns conditional statemes if possible)

[//]: # (6. Use 'and', 'or' or similar keywords in conditional statements if possible)

[//]: # (7. We should have ability to catch exceptions in filter/exception handler instead of having try catch blocks everywhere)

[//]: # (8. Keys/storage patterns must be standardized - Use clear hierarchical key patterns &#40;domain:subdomain:resource:{id}:...&#41; &#40;That about redis keys&#41;)

# Cursor Rules for Code Writing (.NET / C#)

1. Prefer `record` (or `record struct`) over `class` when representing immutable data or value semantics.

2. Use the `required` keyword instead of `[Required]` attributes unless explicitly needed for validation frameworks.

3. Always create an interface for services and abstractions.
   Naming convention: `SomethingNew : ISomethingNew`.

4. Use primary constructors when they improve clarity and reduce boilerplate.

5. Prefer pattern matching (`is`, `switch`, relational patterns, etc.) over nested conditionals.

6. Use logical pattern keywords (`and`, `or`, `not`) when they improve readability. Do not sacrifice clarity for compact syntax.

7. Catch only exceptions that can be handled meaningfully.
   Use exception filters (`when`) when appropriate.
   Avoid scattering `try/catch` blocks.
   Use centralized middleware/filters for cross-cutting exception handling.

8. Redis keys must follow a standardized hierarchical format:
   `domain:subdomain:resource:{id}:...`

9. All I/O-bound methods must accept a `CancellationToken`.

10. All asynchronous methods must use the `Async` suffix.

11. Never use `new HttpClient()`. Always use `IHttpClientFactory`.

12. Choose service lifetimes intentionally: `Singleton`, `Scoped`, `Transient`. Avoid lifetime mismatches.

13. Do not inject `IServiceProvider` or implement service locator patterns unless absolutely necessary.

14. Prefer immutability by default. Types should be immutable unless mutation is explicitly required.

15. Use `init` setters instead of `set` where post-construction mutation is not required.

16. Expose collections as `IReadOnlyList<T>`, `IReadOnlyCollection<T>`, or `ImmutableArray<T>` when appropriate.
    Do not expose mutable collections directly.

17. For lookup-heavy operations, prefer `Dictionary<TKey, TValue>` or `HashSet<T>` over repeated `Any()` or `Contains()` on lists in performance-sensitive paths.

18. Use consistent suffixes: `Repository`, `Service`, `Handler`, `Factory`, `Mapper`.

19. Do not perform DTO ↔ Domain mapping inside controllers. Use dedicated mapping layers.

20. Use `DateTimeOffset` internally and process/store time in UTC. Avoid `DateTime.Now` in business logic.

21. Define explicit JSON contracts: enforce camelCase, define enum serialization strategy, and avoid relying on implicit serializer defaults.
