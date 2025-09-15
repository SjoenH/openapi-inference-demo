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
}