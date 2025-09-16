# ASP.NET Core: Achieving Perfect OpenAPI Documentation

This project demonstrates how to achieve accurate OpenAPI (Swagger) documentation for ASP.NET Core APIs by comparing traditional controller approaches with modern Minimal API solutions.

## Problem Statement

By default, ASP.NET Core controller-based APIs suffer from poor OpenAPI inference that produces inaccurate documentation:

- **All endpoints documented as returning only "200 OK"** regardless of actual behavior
- **Missing error response documentation** (404, 400, 500, etc.)
- **Incorrect status codes** (DELETE operations return 204 No Content but are documented as 200 OK)
- **Misleading API consumers** who rely on the generated documentation
- **Breaking auto-generated clients** that expect documented behavior

This problem leads to confusion, integration issues, and unreliable API contracts.

## Solution 1: The Minimal API Breakthrough with `TypedResults`

Using `TypedResults` with `Results<>` union types in Minimal APIs provides **automatic and perfect** OpenAPI inference without any manual configuration.

### The Magic Code Pattern

```csharp
using Microsoft.AspNetCore.Http.HttpResults;

// ✅ Perfect OpenAPI inference with TypedResults
app.MapDelete("/demo/{id}", Results<NoContent, NotFound> (int id) =>
{
    if (id < 1) return TypedResults.NotFound();    // Automatically inferred as 404
    return TypedResults.NoContent();               // Automatically inferred as 204
});

// ✅ OpenAPI documentation shows "204 No Content" and "404 Not Found"
```

### More Complex Example

```csharp
app.MapPost("/demo", Results<Created<DemoDto>, BadRequest<string>> (DemoDto item) =>
{
    if (string.IsNullOrEmpty(item.Name))
        return TypedResults.BadRequest("Name is required");
    
    var created = new DemoDto { Id = 123, Name = item.Name, CreatedAt = DateTime.UtcNow };
    return TypedResults.Created($"/demo/{created.Id}", created);
});
```

### Why This Works

The union type `Results<NoContent, NotFound>` creates a **transparent, statically analyzable contract** that the OpenAPI generator can automatically understand and document correctly. The type system enforces that only the specified return types can be used, ensuring documentation accuracy.

## Solution 2: The Controller API Fix with Attributes and Analyzers

To achieve the same result in traditional controllers, developers must be **explicit** using the `[ProducesResponseType]` attribute.

### The Required Attributes

The `[ProducesResponseType]` attribute is the primary tool for fixing controller OpenAPI documentation:

```csharp
[HttpDelete("{id}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public IActionResult Delete(int id)
{
    if (id < 1) return NotFound();
    return NoContent();
}
```

### Before and After Example

**❌ Before (Poor Documentation):**
```csharp
[HttpDelete("{id}")]
public IActionResult Delete(int id)
{
    if (id < 1) return NotFound();
    return NoContent();
}
// OpenAPI shows only "200 Success"
```

**✅ After (Accurate Documentation):**
```csharp
[HttpDelete("{id}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public IActionResult Delete(int id)
{
    if (id < 1) return NotFound();
    return NoContent();
}
// OpenAPI shows "204 No Content" and "404 Not Found"
```

### Enforcement with Analyzers

The `Microsoft.AspNetCore.Mvc.Api.Analyzers` NuGet package can be used for **enforcing** this rule at compile time, preventing developers from forgetting the attributes:

```xml
<PackageReference Include="Microsoft.AspNetCore.Mvc.Api.Analyzers" Version="3.0.0-preview3-19153-02" />
```

This analyzer provides compile-time warnings when controller actions are missing proper `[ProducesResponseType]` attributes. Note that as of .NET 8, this analyzer is in preview and primarily targets earlier versions of .NET Core.

## Comparative Analysis

| Aspect | Minimal API with TypedResults | Controller with Attributes |
|--------|-------------------------------|---------------------------|
| **Mechanism** | Automatic Inference via Union Types | Explicit Attributes |
| **Verbosity** | Low - Built into return type | Medium to High - Separate attributes required |
| **Enforcement** | Built into the Type System | Requires Roslyn Analyzer |
| **Refactoring Safety** | High - Compile error if return type is wrong | Lower - Easy to forget an attribute |
| **Documentation Accuracy** | Guaranteed by type system | Depends on developer discipline |
| **Learning Curve** | Moderate - New concepts | Low - Familiar attribute pattern |
| **Legacy Compatibility** | Requires Minimal API adoption | Works with existing controllers |

## Conclusion & Recommendation

Both approaches can achieve perfect OpenAPI documentation, but they differ significantly in their implementation philosophy:

### Key Finding
**Minimal APIs with `TypedResults` provide superior automatic inference** because the type system itself enforces the contract, while controller-based approaches rely on manual annotation and external tooling.

### Recommendations

- **For new development**: Use **Minimal APIs with `TypedResults`** due to their superior type safety, automatic inference, and reduced maintenance burden.

- **For existing controller-based applications**: Adopt the pattern of explicit `[ProducesResponseType]` attributes enforced by the `Microsoft.AspNetCore.Mvc.Api.Analyzers` package to achieve accurate documentation.

- **Migration strategy**: Consider gradually migrating critical endpoints to Minimal APIs while fixing remaining controllers with proper attributes.

## How to Run the Demo

Follow these steps to see the differences in action:

```bash
# Clone the repository
git clone https://github.com/SjoenH/openapi-inference-demo.git
cd openapi-inference-demo

# Run the application
dotnet run

# Open Swagger UI to compare all approaches
# Navigate to: http://localhost:5076/swagger
```

### What to Compare in Swagger UI

1. **Demo Controller** (`/Demo/*`) - Shows the problem: only "200 Success" responses
2. **TypedResponse Controller** (`/api/TypedResponse/*`) - Shows the fix: proper response codes with attributes
3. **MinimalApiDemo** (`/minimal-api/demo/*`) - Shows the breakthrough: automatic accurate documentation

Look specifically at the DELETE endpoints to see the difference:
- Traditional controller: Only "200 Success"
- Fixed controller: "204 No Content" and "404 Not Found" 
- Minimal API: "204 No Content" and "404 Not Found" (automatic)

The demo proves that **TypedResults with `Results<>` union types** provides the most elegant solution for achieving perfect OpenAPI documentation in ASP.NET Core applications.