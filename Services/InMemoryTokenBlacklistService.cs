namespace SafeScribe.JwtApi.Services;
public class InMemoryTokenBlacklistService : ITokenBlacklistService
{
    private readonly HashSet<string> _blacklist = new();
    public Task AddToBlacklistAsync(string jti)
    {
        lock(_blacklist) { _blacklist.Add(jti); }
        return Task.CompletedTask;
    }
    public Task<bool> IsBlacklistedAsync(string jti)
    {
        lock(_blacklist) { return Task.FromResult(_blacklist.Contains(jti)); }
    }
}
