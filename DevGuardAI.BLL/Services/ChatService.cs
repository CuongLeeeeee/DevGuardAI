using DevGuardAI.DAL.Entities;
using DevGuardAI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

public class ChatService : IChatService
{
    private readonly IChatSessionsRepository _sessionRepo;
    private readonly IChatMessagesRepository _messageRepo;
    public ChatService(
       IChatSessionsRepository sessionRepo,
       IChatMessagesRepository messageRepo)
    {
        _sessionRepo = sessionRepo;
        _messageRepo = messageRepo;
    }


    public async Task<ChatSession> CreateSessionAsync(Guid userId, string sessionType, string title)
    {
        var session = new ChatSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SessionType = sessionType,  // "Review" hoặc "TestCase"
            Title = title,
            ContextSummary = null
        };
        await _sessionRepo.CreateAsync(session);
        return session;
    }

    public async Task<IEnumerable<ChatSession>> GetSessionsByUserAsync(Guid userId, string sessionType)
    {
        return await _sessionRepo.GetQueryable()
            .Where(s => s.UserId == userId && s.SessionType == sessionType)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<ChatSession?> GetSessionWithMessagesAsync(Guid sessionId)
    {
        // Dùng GetQueryable để Include messages, lấy 6 gần nhất
        return await _sessionRepo.GetQueryable()
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    public async Task SaveTurnAsync(Guid sessionId, string userContent, string aiContent)
    {
        await _messageRepo.CreateAsync(new ChatMessage
        {
            Id = Guid.NewGuid(),
            ChatSessionId = sessionId,
            Role = "User",
            Content = userContent
        });

        await _messageRepo.CreateAsync(new ChatMessage
        {
            Id = Guid.NewGuid(),
            ChatSessionId = sessionId,
            Role = "AI",
            Content = aiContent
        });
    }

    public async Task UpdateContextSummaryAsync(Guid sessionId, string newSummary)
    {
        var session = await _sessionRepo.GetByIdAsync(sessionId);
        if (session == null) return;

        session.ContextSummary = newSummary;
        await _sessionRepo.UpdateAsync(sessionId, session);
    }
}