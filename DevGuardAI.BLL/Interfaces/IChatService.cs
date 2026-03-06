using DevGuardAI.DAL.Entities;

public interface IChatService
{
    // Tạo session mới khi user bắt đầu conversation
    Task<ChatSession> CreateSessionAsync(Guid userId, string sessionType, string title);

    // Lấy session + messages để build context
    Task<ChatSession?> GetSessionWithMessagesAsync(Guid sessionId);

    // Lưu 1 cặp message (user + AI) sau mỗi turn
    Task SaveTurnAsync(Guid sessionId, string userContent, string aiContent);

    // Cập nhật ContextSummary sau mỗi turn
    Task UpdateContextSummaryAsync(Guid sessionId, string newSummary);
    Task<IEnumerable<ChatSession>> GetSessionsByUserAsync(Guid userId, string sessionType);

}