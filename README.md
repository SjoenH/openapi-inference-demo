# OpenAPI Inference Demo

This project demonstrates the significant limitations of OpenAPI automatic inference in ASP.NET Core WebAPI applications. While ASP.NET Core can automatically generate OpenAPI documentation from controller methods without explicit `[ProducesResponseType]` attributes, the inference is severely limited and often inaccurate.

## 🚨 Key Findings: OpenAPI Inference Limitations

### ✅ What OpenAPI Inference Gets Right
- **200 OK responses** with typed return values (e.g., `ActionResult<T>`) are correctly inferred
- **Request body schemas** are properly documented from method parameters
- **Route parameters** are correctly identified and documented

### ❌ Critical Limitations Found

#### 1. Missing Error Response Documentation
OpenAPI inference **completely ignores** error responses returned by controller actions:
- `NotFound()` → **Not documented**
- `BadRequest()` → **Not documented** 
- `Unauthorized()` → **Not documented**
- `Forbid()` → **Not documented**
- `Conflict()` → **Not documented**
- `UnprocessableEntity()` → **Not documented**

#### 2. Incorrect Status Code Inference
Actions that return specific status codes are **incorrectly documented as "200 OK"**:
- `NoContent()` (204) → **Documented as 200 OK**
- `CreatedAtAction()` (201) → **Documented as 200 OK**
- `Accepted()` (202) → **Documented as 200 OK**
- All other non-200 status codes → **Documented as 200 OK**

#### 3. Complex Logic Ignored
Methods with conditional logic that can return multiple status codes only show **200 OK** regardless of the actual response scenarios implemented.

## 📊 Test Results Summary

Our comprehensive testing revealed that **ALL endpoints show only "200 OK"** in Swagger UI, regardless of their actual behavior:

| Endpoint | Actual Behavior | Swagger Documentation |
|----------|----------------|----------------------|
| `GET /demo/{id}` | 200 OK with data, 404 Not Found | ✅ 200 OK only |
| `POST /demo` | 201 Created, 400 Bad Request | ❌ 200 OK only |
| `PUT /demo/{id}` | 200 OK, 400 Bad Request, 403 Forbidden, 404 Not Found | ❌ 200 OK only |
| `DELETE /demo/{id}` | 204 No Content, 404 Not Found | ❌ 200 OK only |
| `GET /demo/status/{code}` | 200, 201, 202, 204, 400, 401, 403, 404, 409, 422, 500 | ❌ 200 OK only |
| `POST /demo/validation` | 200 OK, 400, 403, 409, 422 | ❌ 200 OK only |

## 🧪 Demonstration Endpoints

### Basic CRUD Operations
- **GET /demo/{id}** - Returns 200 with data or 404 for invalid IDs
- **POST /demo** - Returns 201 Created or 400 for invalid data
- **PUT /demo/{id}** - Returns 200, 400, 403 (for system items), or 404
- **DELETE /demo/{id}** - Returns 204 No Content or 404

### Status Code Testing Endpoint
- **GET /demo/status/{code}** - Explicitly returns the requested HTTP status code:
  - 200, 201, 202, 204, 400, 401, 403, 404, 409, 422, 500

### Validation Testing Endpoint  
- **POST /demo/validation** - Complex validation with multiple response types:
  - 200 for valid data
  - 400 for missing request body
  - 422 for validation failures (empty name, too long, whitespace)
  - 403 for reserved names ("admin")
  - 409 for conflicting names ("conflict")

## 🖼️ Visual Evidence

![OpenAPI Inference Limitations in Swagger UI](https://github.com/user-attachments/assets/00dd3fa6-7510-45ac-9d4c-a61d88074c57)

The screenshot above shows multiple endpoints in Swagger UI, all incorrectly documented as returning only "200 OK" despite their actual diverse response behaviors.

## 🔧 Running the Demo

### Prerequisites
- .NET 8.0 SDK
- Your favorite HTTP client (curl, Postman, etc.)

### Quick Start
```bash
# Clone and run
git clone <repository-url>
cd openapi-inference-demo
dotnet run

# Open Swagger UI
# Navigate to: http://localhost:5076/swagger
```

### Testing Endpoints
The project includes a comprehensive HTTP test file (`OpenApiInferenceDemo.http`) with examples for all scenarios:

```bash
# Test 204 No Content (shows as 200 in Swagger)
curl -v http://localhost:5076/demo/5 -X DELETE

# Test 201 Created (shows as 200 in Swagger)
curl -v http://localhost:5076/demo -X POST -H "Content-Type: application/json" -d '{"name":"Test"}'

# Test specific status codes
curl -v http://localhost:5076/demo/status/204
curl -v http://localhost:5076/demo/status/422
curl -v http://localhost:5076/demo/status/500
```

## 💡 Implications for API Development

These limitations have serious consequences:

1. **Misleading Documentation** - Clients expect only 200 responses based on Swagger UI
2. **Poor Developer Experience** - Frontend developers can't prepare for error scenarios
3. **Testing Gaps** - Automated API testing tools miss error cases
4. **Integration Issues** - Third-party integrations fail to handle error responses properly

## 🎯 Best Practices

**Always use explicit response type attributes** for production APIs:

```csharp
[HttpGet("{id}")]
[ProducesResponseType<DemoDto>(200)]
[ProducesResponseType<ProblemDetails>(404)]
public ActionResult<DemoDto> Get(int id)
{
    // Implementation
}
```

## 📋 Project Structure

```
├── Controllers/
│   └── DemoController.cs          # Main controller with all test endpoints
├── DemoDto.cs                     # Data transfer object
├── Program.cs                     # Application configuration
├── OpenApiInferenceDemo.http      # HTTP test file with all scenarios
├── OpenApiInferenceDemo.csproj    # Project file
└── README.md                      # This comprehensive documentation
```

## 🎯 Conclusion

This demo conclusively proves that **automatic OpenAPI inference in ASP.NET Core is inadequate for production use**. While convenient for rapid prototyping, it provides dangerously incomplete and inaccurate API documentation. 

For any serious API development, explicit response type attributes are essential to ensure accurate, complete, and reliable OpenAPI documentation.