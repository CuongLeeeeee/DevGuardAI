using DevGuardAI.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevGuardAI.DAL.Data;

public class DevGuardAIDbContext : DbContext
{
    public DevGuardAIDbContext(DbContextOptions<DevGuardAIDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ChatSession> ChatSessions { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // USER
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.HasMany(u => u.ChatSessions)
                  .WithOne(c => c.User)
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // CHAT SESSION
        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.HasMany(c => c.Messages)
                  .WithOne(m => m.ChatSession)
                  .HasForeignKey(m => m.ChatSessionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // CHAT MESSAGE
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(m => m.Id);
        });
    }
}