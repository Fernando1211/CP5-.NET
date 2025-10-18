using SafeScribe.JwtApi.Models;
namespace SafeScribe.JwtApi;
public class InMemoryNoteRepository : INoteRepository
{
    private readonly List<Note> _notes = new();
    public Task AddAsync(Note note)
    {
        _notes.Add(note);
        return Task.CompletedTask;
    }
    public Task<Note?> GetByIdAsync(Guid id) =>
        Task.FromResult(_notes.FirstOrDefault(n => n.Id == id));
    public Task UpdateAsync(Note note)
    {
        var idx = _notes.FindIndex(n => n.Id == note.Id);
        if (idx >= 0) _notes[idx] = note;
        return Task.CompletedTask;
    }
    public Task DeleteAsync(Guid id)
    {
        _notes.RemoveAll(n => n.Id == id);
        return Task.CompletedTask;
    }
    public Task<IEnumerable<Note>> GetAllAsync() => Task.FromResult<IEnumerable<Note>>(_notes);
}
