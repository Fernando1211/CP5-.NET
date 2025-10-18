using SafeScribe.JwtApi.Models;
namespace SafeScribe.JwtApi.Services;
public interface ITokenService
{
    Task<string> CreateTokenAsync(User user);
    Task<User?> ValidateCredentialsAsync(string username, string password);
    Task RegisterUserAsync(User user, string password);
}
