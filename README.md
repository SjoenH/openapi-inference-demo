# OpenAPI Inference Demo

This project demonstrates the superior OpenAPI inference capabilities of ASP.NET Core minimal APIs with TypedResults compared to traditional controller-based APIs.

## 🎯 Problem Investigated

Traditional controller-based APIs suffer from severe OpenAPI inference limitations:
- All endpoints documented as returning only "200 OK" regardless of actual behavior
- Missing error response documentation (404, 400, 500, etc.)
- Incorrect status codes (DELETE returns 204 but documented as 200)

## 🎉 BREAKTHROUGH: TypedResults Solution!

**Key Discovery**: Using `TypedResults` with `Results<>` union types in minimal APIs provides **automatic OpenAPI inference** without manual annotations!

### TypedResults Syntax
```csharp
// ✅ Perfect OpenAPI inference with TypedResults
group.MapDelete("/{id}", Results<NoContent, NotFound> (int id) =>
{
    if (id < 1) return TypedResults.NotFound();    // Inferred as 404
    return TypedResults.NoContent();               // Inferred as 204
});
// ✅ OpenAPI documents as "204 No Content" and "404 Not Found"
```

## 📊 Comprehensive Comparison Results

| Endpoint | Actual Behavior | Traditional Controller | Strongly Typed Controller | Minimal API (TypedResults) |
|----------|----------------|------------------------|---------------------------|---------------------------|
| `DELETE /{id}` | 204 No Content, 404 Not Found | ❌ "200 Success" only | ❌ "200 Success" only | ✅ "204 No Content", "404 Not Found" |
| `POST /` | 201 Created, 400 Bad Request | ❌ "200 Success" only | ❌ "200 Success" only | ✅ "201 Created", "400 Bad Request" |
| `GET /{id}` | 200 OK, 404 Not Found | ❌ "200 Success" only | ❌ "200 Success" only | ✅ "200 OK", "404 Not Found" |

**Key Finding**: Even strongly typed controllers with explicit `Ok()`, `BadRequest()`, and `NotFound()` calls still fail to provide automatic OpenAPI inference. Only TypedResults with `Results<>` union types achieves perfect inference.

## 🔍 Three API Approaches Tested

### 1. Traditional Controller (`/Demo/*`)
```csharp
[HttpDelete("{id}")]
public ActionResult Delete(int id)
{
    if (id < 1) return NotFound();
    return NoContent();
}
// ❌ OpenAPI shows only "200 Success"
```

### 2. Strongly Typed Controller (`/api/TypedResponse/*`)
```csharp
[HttpDelete("{id}")]
public IActionResult Delete(int id)
{
    if (id < 1) return NotFound();
    return NoContent();
}
// ❌ Still shows only "200 Success"
```

### 3. Minimal API with TypedResults (`/minimal-api/demo/*`)
```csharp
group.MapDelete("/{id}", Results<NoContent, NotFound> (int id) =>
{
    if (id < 1) return TypedResults.NotFound();
    return TypedResults.NoContent();
});
// ✅ Shows "204 No Content" and "404 Not Found"
```

## 🎯 Key Findings

1. **TypedResults + Results<> Union Types = Perfect Inference**: This combination provides automatic status code inference
2. **Traditional Results Class Fails**: Both controllers and regular `Results` class show identical poor inference 
3. **Manual Annotations Not Required**: TypedResults eliminates the need for `.Produces()` or `[ProducesResponseType]`
4. **Type Safety**: Results union types provide compile-time type safety for return types
5. **Endpoints Work Correctly**: All endpoints return proper status codes AND documentation reflects this

## 🔑 Solution Pattern

```csharp
using Microsoft.AspNetCore.Http.HttpResults;

// TypedResults with Results<> union types
group.MapPost("/", Results<Created<DemoDto>, BadRequest<string>> (DemoDto item) =>
{
    if (string.IsNullOrEmpty(item.Name))
        return TypedResults.BadRequest("Name is required");
    
    var created = new DemoDto { /* ... */ };
    return TypedResults.Created($"/demo/{created.Id}", created);
});
```

## 🚀 Running the Demo

```bash
git clone <repository-url>
cd openapi-inference-demo
dotnet run

# Open Swagger UI to compare all three approaches
open http://localhost:5076/swagger
```

## 🎯 Conclusion

**TypedResults with Results<> union types** in minimal APIs provides the breakthrough solution for automatic OpenAPI inference in ASP.NET Core. This approach:

- ✅ Eliminates manual annotations
- ✅ Provides accurate status code documentation  
- ✅ Includes proper response schemas
- ✅ Offers compile-time type safety
- ✅ Works without any configuration changes

The investigation proves that minimal APIs with TypedResults **do provide superior automatic OpenAPI inference** compared to traditional controller-based approaches.