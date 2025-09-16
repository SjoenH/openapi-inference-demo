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
    /// Get a demo item by ID - FIXED with proper ProducesResponseType attributes
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType<DemoDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// Create a new demo item - FIXED with proper ProducesResponseType attributes
    /// </summary>
    [HttpPost]
    [ProducesResponseType<DemoDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
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
    /// Delete a demo item - FIXED with proper ProducesResponseType attributes
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        if (id < 1)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Update a demo item - FIXED with proper ProducesResponseType attributes
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType<DemoDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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