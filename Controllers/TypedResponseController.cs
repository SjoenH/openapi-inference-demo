using Microsoft.AspNetCore.Mvc;

namespace OpenApiInferenceDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TypedResponseController : ControllerBase
{
    private readonly ILogger<TypedResponseController> _logger;

    public TypedResponseController(ILogger<TypedResponseController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get a demo item by ID using strongly typed responses (similar to TypedResults approach)
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<DemoDto> Get(int id)
    {
        if (id < 1)
        {
            return NotFound();
        }

        return Ok(new DemoDto
        {
            Id = id,
            Name = $"Demo Item {id}",
            CreatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Create a new demo item using strongly typed responses (similar to TypedResults approach)
    /// </summary>
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

        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    /// <summary>
    /// Delete a demo item using strongly typed responses (similar to TypedResults approach)
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id < 1)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Update a demo item using strongly typed responses (similar to TypedResults approach)
    /// </summary>
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
            return Forbid();
        }

        return Ok(new DemoDto
        {
            Id = id,
            Name = item.Name,
            CreatedAt = DateTime.UtcNow
        });
    }
}