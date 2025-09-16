using Microsoft.AspNetCore.Http.HttpResults;

namespace OpenApiInferenceDemo;

public static class MinimalApiEndpoints
{
    public static void MapDemoMinimalApiEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/minimal-api/demo")
            .WithTags("MinimalApiDemo")
            .WithDisplayName("Minimal API Demo Endpoints with TypedResults");

        // GET /minimal-api/demo/{id} - Testing TypedResults for automatic OpenAPI inference
        group.MapGet("/{id}", Results<Ok<DemoDto>, NotFound> (int id) =>
        {
            if (id < 1)
            {
                return TypedResults.NotFound();
            }

            var result = new DemoDto
            {
                Id = id,
                Name = $"Demo Item {id}",
                CreatedAt = DateTime.UtcNow
            };
            return TypedResults.Ok(result);
        });

        // POST /minimal-api/demo - Testing TypedResults for automatic OpenAPI inference
        group.MapPost("/", Results<Created<DemoDto>, BadRequest<string>> (DemoDto item) =>
        {
            if (string.IsNullOrEmpty(item.Name))
            {
                return TypedResults.BadRequest("Name is required");
            }

            var created = new DemoDto
            {
                Id = Random.Shared.Next(1, 1000),
                Name = item.Name,
                CreatedAt = DateTime.UtcNow
            };

            // Return 201 Created with location header
            return TypedResults.Created($"/minimal-api/demo/{created.Id}", created);
        });

        // PUT /minimal-api/demo/{id} - Testing TypedResults for automatic OpenAPI inference
        group.MapPut("/{id}", Results<Ok<DemoDto>, NotFound, BadRequest<string>, ForbidHttpResult> (int id, DemoDto item) =>
        {
            if (id < 1)
            {
                return TypedResults.NotFound();
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                return TypedResults.BadRequest("Name is required");
            }

            if (id == 999)
            {
                return TypedResults.Forbid();
            }

            var result = new DemoDto
            {
                Id = id,
                Name = item.Name,
                CreatedAt = DateTime.UtcNow
            };

            return TypedResults.Ok(result);
        });

        // DELETE /minimal-api/demo/{id} - Testing TypedResults for automatic OpenAPI inference
        group.MapDelete("/{id}", Results<NoContent, NotFound> (int id) =>
        {
            if (id < 1)
            {
                return TypedResults.NotFound();
            }

            // Return 204 No Content
            return TypedResults.NoContent();
        });

        // POST /minimal-api/demo/validation - Testing TypedResults for automatic OpenAPI inference
        group.MapPost("/validation", Results<Ok<DemoDto>, BadRequest<string>, UnprocessableEntity<string>> (DemoDto item) =>
        {
            if (item == null)
            {
                return TypedResults.BadRequest("Request body is required");
            }

            if (string.IsNullOrWhiteSpace(item.Name))
            {
                return TypedResults.UnprocessableEntity("Name cannot be empty or whitespace");
            }

            if (item.Name.Length > 50)
            {
                return TypedResults.UnprocessableEntity("Name cannot exceed 50 characters");
            }

            var result = new DemoDto
            {
                Id = Random.Shared.Next(1, 1000),
                Name = item.Name,
                CreatedAt = DateTime.UtcNow
            };

            return TypedResults.Ok(result);
        });
    }

    public static void MapDemoMinimalApiEndpointsWithoutExplicitTypes(this WebApplication app)
    {
        var group = app.MapGroup("/minimal-api/no-explicit-types")
            .WithTags("MinimalApiDemo-NoExplicitTypes")
            .WithDisplayName("Minimal API Demo WITHOUT Explicit Results<> Types");

        // GET /minimal-api/no-explicit-types/{id} - Testing TypedResults WITHOUT explicit Results<> types
        group.MapGet("/{id}", GetItemWithoutExplicitTypes);

        // POST /minimal-api/no-explicit-types - Testing TypedResults WITHOUT explicit Results<> types
        group.MapPost("/", CreateItemWithoutExplicitTypes);

        // PUT /minimal-api/no-explicit-types/{id} - Testing TypedResults WITHOUT explicit Results<> types
        group.MapPut("/{id}", UpdateItemWithoutExplicitTypes);

        // DELETE /minimal-api/no-explicit-types/{id} - Testing TypedResults WITHOUT explicit Results<> types
        group.MapDelete("/{id}", DeleteItemWithoutExplicitTypes);

        // POST /minimal-api/no-explicit-types/validation - Testing TypedResults WITHOUT explicit Results<> types
        group.MapPost("/validation", ValidateItemWithoutExplicitTypes);
    }

    // Method definitions for endpoints without explicit Results<> types
    private static IResult GetItemWithoutExplicitTypes(int id)
    {
        if (id < 1)
        {
            return TypedResults.NotFound();
        }

        var result = new DemoDto
        {
            Id = id,
            Name = $"Demo Item {id}",
            CreatedAt = DateTime.UtcNow
        };
        return TypedResults.Ok(result);
    }

    private static IResult CreateItemWithoutExplicitTypes(DemoDto item)
    {
        if (string.IsNullOrEmpty(item.Name))
        {
            return TypedResults.BadRequest("Name is required");
        }

        var created = new DemoDto
        {
            Id = Random.Shared.Next(1, 1000),
            Name = item.Name,
            CreatedAt = DateTime.UtcNow
        };

        return TypedResults.Created($"/minimal-api/no-explicit-types/{created.Id}", created);
    }

    private static IResult UpdateItemWithoutExplicitTypes(int id, DemoDto item)
    {
        if (id < 1)
        {
            return TypedResults.NotFound();
        }

        if (string.IsNullOrEmpty(item.Name))
        {
            return TypedResults.BadRequest("Name is required");
        }

        if (id == 999)
        {
            return TypedResults.Forbid();
        }

        var result = new DemoDto
        {
            Id = id,
            Name = item.Name,
            CreatedAt = DateTime.UtcNow
        };

        return TypedResults.Ok(result);
    }

    private static IResult DeleteItemWithoutExplicitTypes(int id)
    {
        if (id < 1)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }

    private static IResult ValidateItemWithoutExplicitTypes(DemoDto item)
    {
        if (item == null)
        {
            return TypedResults.BadRequest("Request body is required");
        }

        if (string.IsNullOrWhiteSpace(item.Name))
        {
            return TypedResults.UnprocessableEntity("Name cannot be empty or whitespace");
        }

        if (item.Name.Length > 50)
        {
            return TypedResults.UnprocessableEntity("Name cannot exceed 50 characters");
        }

        var result = new DemoDto
        {
            Id = Random.Shared.Next(1, 1000),
            Name = item.Name,
            CreatedAt = DateTime.UtcNow
        };

        return TypedResults.Ok(result);
    }
}