using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/testcase")]
public class TestCaseController : ControllerBase
{
    private readonly IGeminiService _service;
    private readonly IChatService _chatService;

    public TestCaseController(IGeminiService service, IChatService chatService)
    {
        _service = service;
        _chatService = chatService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(ContentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Code is required");

        var result = await _service.GenerateTestCases(request.Content);

        return Ok(result);
    }

    [HttpPost("conversation")]
    [Authorize]
    public async Task<IActionResult> GenerateConversation([FromBody] ConversationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Code is required");

        var userId = GetUserIdFromToken();

        // Verify session thuộc về user đang login
        var session = await _chatService.GetSessionWithMessagesAsync(request.SessionId);
        if (session == null || session.UserId != userId)
            return Forbid();

        var result = await _service.GenerateTestCasesWithContext(request.SessionId, request.Content);
        return Ok(result);
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