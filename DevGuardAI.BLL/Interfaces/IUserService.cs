public interface IUserService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
}