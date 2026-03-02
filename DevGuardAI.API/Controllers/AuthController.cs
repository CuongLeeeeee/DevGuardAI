using DevGuardAI.DAL.Entities;
using DevGuardAI.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;

    public AuthController(
        IUserRepository userRepository,
        JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check email existing
        var existingUser = await _userRepository
            .GetByEmailAsync(request.Email);

        if (existingUser != null)
            return BadRequest(new { message = "Email already exists." });

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        var token = _jwtService.GenerateToken(user.Id, user.Email);

        return Ok(new AuthResponse
        {
            Email = user.Email,
            Token = token
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userRepository
            .GetByEmailAsync(request.Email);

        if (user == null)
            return Unauthorized(new { message = "Invalid email or password." });

        var isValidPassword = BCrypt.Net.BCrypt
            .Verify(request.Password, user.PasswordHash);

        if (!isValidPassword)
            return Unauthorized(new { message = "Invalid email or password." });

        var token = _jwtService.GenerateToken(user.Id, user.Email);

        return Ok(new AuthResponse
        {
            Email = user.Email,
            Token = token
        });
    }
}