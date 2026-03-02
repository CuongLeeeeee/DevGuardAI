using BCrypt.Net;
using DevGuardAI.DAL.Entities;
using DevGuardAI.DAL.Interfaces;
using DevGuardAI.DAL.Repositories;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly JwtService _jwtService;

    public UserService(
        IUserRepository userRepo,
        JwtService jwtService)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check email tồn tại
        var existing = await _userRepo.FindAsync(x => x.Email == request.Email);
        if (existing.Any())
            throw new Exception("Email already exists");

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepo.CreateAsync(user);

        var token = _jwtService.GenerateToken(user.Id, user.Email);

        return new AuthResponse
        {
            Token = token,
            Email = user.Email
        };
    }
}