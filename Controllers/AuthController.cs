using Microsoft.AspNetCore.Mvc;
using SafeScribe.JwtApi.DTOs;
using SafeScribe.JwtApi.Models;
using SafeScribe.JwtApi.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace SafeScribe.JwtApi.Controllers;
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ITokenBlacklistService _blacklist;

    public AuthController(ITokenService tokenService, ITokenBlacklistService blacklist)
    {
        _tokenService = tokenService;
        _blacklist = blacklist;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var existing = await _tokenService.ValidateCredentialsAsync(dto.Username, dto.Password);
        if (existing != null)
            return Conflict(new { message = "Usuário já existe ou credenciais inválidas." });

        var user = new User
        {
            Username = dto.Username,
            Role = dto.Role
        };
        await _tokenService.RegisterUserAsync(user, dto.Password);
        return CreatedAtAction(null, new { user.Id, user.Username, user.Role });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var user = await _tokenService.ValidateCredentialsAsync(dto.Username, dto.Password);
        if (user == null) return Unauthorized(new { message = "Credenciais inválidas." });

        var token = await _tokenService.CreateTokenAsync(user);
        return Ok(new { token });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        if (string.IsNullOrEmpty(jti)) return BadRequest(new { message = "Token não contém jti." });
        await _blacklist.AddToBlacklistAsync(jti);
        return Ok(new { message = "Logout efetuado." });
    }
}
