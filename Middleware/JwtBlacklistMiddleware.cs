using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using SafeScribe.JwtApi.Services;

namespace SafeScribe.JwtApi.Middleware;
public class JwtBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public JwtBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenBlacklistService blacklist)
    {
        // If user is authenticated, extract jti claim and check blacklist
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var jti = context.User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;
            if (!string.IsNullOrEmpty(jti))
            {
                var isBlacklisted = await blacklist.IsBlacklistedAsync(jti);
                if (isBlacklisted)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token invalidated (blacklisted).");
                    return;
                }
            }
        }

        await _next(context);
    }
}
