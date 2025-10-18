namespace SafeScribe.JwtApi.Models;
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!; // nunca armazene senha em texto plano
    public string Role { get; set; } = Roles.Reader;
}
