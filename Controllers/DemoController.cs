using Microsoft.AspNetCore.Mvc;

namespace OpenApiInferenceDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class DemoController : ControllerBase
{
    private readonly ILogger<DemoController> _logger;

    public DemoController(ILogger<DemoController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public ActionResult<DemoDto> Get(int id)
    {
        if (id < 1)
        {
            return NotFound();
        }

        return new DemoDto
        {
            Id = id,
            Name = $"Demo Item {id}",
            CreatedAt = DateTime.UtcNow
        };
    }

    [HttpPost]
    public ActionResult<DemoDto> Create(DemoDto item)
    {
        if (string.IsNullOrEmpty(item.Name))
        {
            return BadRequest("Name is required");
        }

        var created = new DemoDto
        {
            Id = Random.Shared.Next(1, 1000),
            Name = item.Name,
            CreatedAt = DateTime.UtcNow
        };

        // Return 201 Created with location header
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public ActionResult<DemoDto> Update(int id, DemoDto item)
    {
        if (id < 1)
        {
            return NotFound();
        }

        if (string.IsNullOrEmpty(item.Name))
        {
            return BadRequest("Name is required");
        }

        if (id == 999)
        {
            return Forbid("Cannot update system item");
        }

        return new DemoDto
        {
            Id = id,
            Name = item.Name,
            CreatedAt = DateTime.UtcNow
        };
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        if (id < 1)
        {
            return NotFound();
        }

        // Simulate deletion - return 204 No Content
        return NoContent();
    }

    [HttpGet("status/{code}")]
    public ActionResult GetWithStatusCode(int code)
    {
        return code switch
        {
            200 => Ok(new { message = "Success", timestamp = DateTime.UtcNow }),
            201 => Created("/demo/123", new { id = 123, message = "Created" }),
            202 => Accepted(new { message = "Accepted for processing" }),
            204 => NoContent(),
            400 => BadRequest("Bad request example"),
            401 => Unauthorized(),
            403 => Forbid("Access denied"),
            404 => NotFound("Resource not found"),
            409 => Conflict("Resource already exists"),
            422 => UnprocessableEntity("Validation failed"),
            500 => Problem("Internal server error", statusCode: 500),
            _ => BadRequest("Unsupported status code")
        };
    }

    [HttpPost("validation")]
    public ActionResult<DemoDto> ValidateAndCreate(DemoDto item)
    {
        if (item == null)
        {
            return BadRequest("Request body is required");
        }

        if (string.IsNullOrWhiteSpace(item.Name))
        {
            return UnprocessableEntity("Name cannot be empty or whitespace");
        }

        if (item.Name.Length > 50)
        {
            return UnprocessableEntity("Name cannot exceed 50 characters");
        }

        if (item.Name.ToLower() == "admin")
        {
            return Forbid("Reserved name cannot be used");
        }

        if (item.Name.ToLower() == "conflict")
        {
            return Conflict("Item with this name already exists");
        }

        return new DemoDto
        {
            Id = Random.Shared.Next(1, 1000),
            Name = item.Name,
            CreatedAt = DateTime.UtcNow
        };
    }

    [HttpGet("server-error")]
    public ActionResult GetServerError()
    {
        // Simulate server error
        return Problem("Simulated internal server error", statusCode: 500);
    }

    [HttpGet("accepted")]
    public ActionResult GetAccepted()
    {
        return Accepted(new { message = "Request accepted for processing", jobId = Guid.NewGuid() });
    }
}
