using SafeScribe.JwtApi.Models;
namespace SafeScribe.JwtApi;
public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    public Task AddAsync(User user)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }
    public Task<User?> GetByUsernameAsync(string username) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Username == username));
    public Task<User?> GetByIdAsync(Guid id) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
    public Task<IEnumerable<User>> GetAllAsync() => Task.FromResult<IEnumerable<User>>(_users);
}
