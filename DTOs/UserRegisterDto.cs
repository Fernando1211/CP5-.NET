namespace SafeScribe.JwtApi.DTOs;
public class UserRegisterDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = "Leitor"; // default
}
