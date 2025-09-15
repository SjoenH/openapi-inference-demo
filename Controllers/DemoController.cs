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
}
