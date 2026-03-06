using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/review")]
public class ReviewController : ControllerBase
{
    private readonly IGeminiService _service;
    private readonly IChatService _chatService;

    public ReviewController(IGeminiService service, IChatService chatService)
    {
        _service = service;
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> Review([FromBody] ContentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Content is required");

        var result = await _service.ReviewCode(request.Content);

        return Ok(result);
    }

    // Controller
    [HttpPost("conversation")]
    public async Task<IActionResult> ReviewConversation([FromBody] ConversationRequest request)
    {
        var result = await _service.ReviewWithContext(request.SessionId, request.Content);
        return Ok(result);
    }

}
