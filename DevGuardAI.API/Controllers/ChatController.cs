using DevGuardAI.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("session")]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var session = await _chatService.CreateSessionAsync(userId, request.SessionType, request.Title);
        return Ok(new { session.Id });
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions([FromQuery] string sessionType)
    {
        var userId = GetUserIdFromToken();
        var sessions = await _chatService.GetSessionsByUserAsync(userId, sessionType);
        return Ok(sessions);
    }

    [HttpGet("sessions/{sessionId}/messages")]
    public async Task<IActionResult> GetMessages(Guid sessionId)
    {
        var userId = GetUserIdFromToken();

        var session = await _chatService.GetSessionWithMessagesAsync(sessionId);
        if (session == null || session.UserId != userId)
            return Forbid();

        return Ok(session.Messages
    .OrderBy(m => m.CreatedAt)
    .Select(m => new ChatMessageDto
    {
        Id = m.Id,
        Role = m.Role,
        Content = m.Content,
        CreatedAt = m.CreatedAt
    }));
    }
    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token.");
        }
        return userId;
    }
}