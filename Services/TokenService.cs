using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using SafeScribe.JwtApi.Models;
using BCrypt.Net;

namespace SafeScribe.JwtApi.Services;
public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _users;
    public TokenService(IConfiguration config, IUserRepository users)
    {
        _config = config;
        _users = users;
    }

    public async Task RegisterUserAsync(User user, string password)
    {
        // Hash the password before storing
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        await _users.AddAsync(user);
    }

    public Task<User?> ValidateCredentialsAsync(string username, string password)
    {
        var user = _users.GetByUsernameAsync(username).Result;
        if (user == null) return Task.FromResult<User?>(null);
        var ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        return Task.FromResult(ok ? user : null);
    }

    public Task<string> CreateTokenAsync(User user)
    {
        var jwtSection = _config.GetSection("Jwt");
        var secret = jwtSection.GetValue<string>("Secret")!;
        var issuer = jwtSection.GetValue<string>("Issuer");
        var audience = jwtSection.GetValue<string>("Audience");
        var expiresIn = jwtSection.GetValue<int>("ExpiresInMinutes", 60);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jti = Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, jti)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresIn),
            signingCredentials: creds
        );

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
        return Task.FromResult(tokenStr);
    }
}
