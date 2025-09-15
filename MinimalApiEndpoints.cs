namespace OpenApiInferenceDemo;

public static class MinimalApiEndpoints
{
    public static void MapDemoMinimalApiEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/minimal-api/demo")
            .WithTags("MinimalApiDemo")
            .WithDisplayName("Minimal API Demo Endpoints");

        // GET /minimal-api/demo/{id}
        group.MapGet("/{id}", (int id) =>
        {
            if (id < 1)
            {
                return Results.NotFound();
            }

            var result = new DemoDto
            {
                Id = id,
                Name = $"Demo Item {id}",
                CreatedAt = DateTime.UtcNow
            };
            return Results.Ok(result);
        })
        .Produces<DemoDto>(200)
        .Produces(404);

        // POST /minimal-api/demo
        group.MapPost("/", (DemoDto item) =>
        {
            if (string.IsNullOrEmpty(item.Name))
            {
                return Results.BadRequest("Name is required");
            }

            var created = new DemoDto
            {
                Id = Random.Shared.Next(1, 1000),
                Name = item.Name,
                CreatedAt = DateTime.UtcNow
            };

            // Return 201 Created with location header
            return Results.Created($"/minimal-api/demo/{created.Id}", created);
        })
        .Produces<DemoDto>(201)
        .Produces(400);

        // PUT /minimal-api/demo/{id}
        group.MapPut("/{id}", (int id, DemoDto item) =>
        {
            if (id < 1)
            {
                return Results.NotFound();
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                return Results.BadRequest("Name is required");
            }

            if (id == 999)
            {
                return Results.Forbid();
            }

            var result = new DemoDto
            {
                Id = id,
                Name = item.Name,
                CreatedAt = DateTime.UtcNow
            };

            return Results.Ok(result);
        })
        .Produces<DemoDto>(200)
        .Produces(400)
        .Produces(403)
        .Produces(404);

        // DELETE /minimal-api/demo/{id}
        group.MapDelete("/{id}", (int id) =>
        {
            if (id < 1)
            {
                return Results.NotFound();
            }

            // Return 204 No Content
            return Results.NoContent();
        })
        .Produces(204)
        .Produces(404);

        // GET /minimal-api/demo/status/{code}
        group.MapGet("/status/{code}", (int code) =>
        {
            return code switch
            {
                200 => Results.Ok(new { message = "Success", timestamp = DateTime.UtcNow }),
                201 => Results.Created("/minimal-api/demo/123", new { id = 123, message = "Created" }),
                202 => Results.Accepted("/minimal-api/demo/status", new { message = "Accepted for processing" }),
                204 => Results.NoContent(),
                400 => Results.BadRequest("Bad request example"),
                401 => Results.Unauthorized(),
                403 => Results.Forbid(),
                404 => Results.NotFound("Resource not found"),
                409 => Results.Conflict("Resource already exists"),
                422 => Results.UnprocessableEntity("Validation failed"),
                500 => Results.Problem("Internal server error", statusCode: 500),
                _ => Results.BadRequest("Unsupported status code")
            };
        })
        .Produces(200)
        .Produces(201) 
        .Produces(202)
        .Produces(204)
        .Produces(400)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .Produces(409)
        .Produces(422)
        .Produces(500);

        // POST /minimal-api/demo/validation
        group.MapPost("/validation", (DemoDto item) =>
        {
            if (item == null)
            {
                return Results.BadRequest("Request body is required");
            }

            if (string.IsNullOrWhiteSpace(item.Name))
            {
                return Results.UnprocessableEntity("Name cannot be empty or whitespace");
            }

            if (item.Name.Length > 50)
            {
                return Results.UnprocessableEntity("Name cannot exceed 50 characters");
            }

            if (item.Name.ToLower() == "admin")
            {
                return Results.Forbid();
            }

            if (item.Name.ToLower() == "conflict")
            {
                return Results.Conflict("Item with this name already exists");
            }

            var result = new DemoDto
            {
                Id = Random.Shared.Next(1, 1000),
                Name = item.Name,
                CreatedAt = DateTime.UtcNow
            };

            return Results.Ok(result);
        })
        .Produces<DemoDto>(200)
        .Produces(400)
        .Produces(403)
        .Produces(409)
        .Produces(422);

        // GET /minimal-api/demo/server-error
        group.MapGet("/server-error", () =>
        {
            return Results.Problem("Simulated internal server error", statusCode: 500);
        })
        .Produces(500);

        // GET /minimal-api/demo/accepted
        group.MapGet("/accepted", () =>
        {
            return Results.Accepted("/minimal-api/demo/status", new { message = "Request accepted for processing", jobId = Guid.NewGuid() });
        })
        .Produces(202);
    }
}