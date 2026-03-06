using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/review")]
public class ReviewController : ControllerBase
{
    private readonly IGeminiService _service;

    public ReviewController(IGeminiService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Review([FromBody] ContentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Content is required");

        var result = await _service.ReviewCode(request.Content);

        return Ok(result);
    }

}