using DevGuardAI.BLL.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            throw new ValidationException("content", "Content is required.");

        var result = await _service.ReviewCode(request.Content);
        return Ok(result);
    }

    [HttpPost("conversation")]
    [Authorize]
    public async Task<IActionResult> ReviewConversation([FromBody] ConversationRequest request)
    {
        var userId = GetUserIdFromToken();

        var session = await _chatService.GetSessionWithMessagesAsync(request.SessionId);
        if (session == null)
            throw new NotFoundException("ChatSession", request.SessionId);

        if (session.UserId != userId)
            throw new ForbiddenException();

        var result = await _service.ReviewWithContext(request.SessionId, request.Content);
        return Ok(result);
    }

    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedException("User ID not found in token.");
        return userId;
    }
}