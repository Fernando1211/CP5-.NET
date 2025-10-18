using SafeScribe.JwtApi.Models;
namespace SafeScribe.JwtApi;
public interface INoteRepository
{
    Task AddAsync(Note note);
    Task<Note?> GetByIdAsync(Guid id);
    Task UpdateAsync(Note note);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Note>> GetAllAsync();
}
