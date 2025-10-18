using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SafeScribe.JwtApi.DTOs;
using SafeScribe.JwtApi.Models;
using System.Security.Claims;

namespace SafeScribe.JwtApi.Controllers;
[ApiController]
[Route("api/v1/notas")]
[Authorize]
public class NotasController : ControllerBase
{
    private readonly INoteRepository _notes;
    private readonly IUserRepository _users;

    public NotasController(INoteRepository notes, IUserRepository users)
    {
        _notes = notes;
        _users = users;
    }

    // Create note - requires Editor or Admin
    [HttpPost]
    [Authorize(Roles = "Editor,Admin")]
    public async Task<IActionResult> Create([FromBody] NoteCreateDto dto)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Forbid();

        var note = new Note
        {
            Title = dto.Title,
            Content = dto.Content,
            UserId = userId
        };
        await _notes.AddAsync(note);
        return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
    }

    // Get note
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var note = await _notes.GetByIdAsync(id);
        if (note == null) return NotFound();

        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        Guid.TryParse(userIdStr, out var userId);

        if (role == Roles.Reader || role == Roles.Editor)
        {
            if (note.UserId != userId) return Forbid();
        }

        return Ok(note);
    }

    // Update note
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] NoteCreateDto dto)
    {
        var note = await _notes.GetByIdAsync(id);
        if (note == null) return NotFound();

        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        Guid.TryParse(userIdStr, out var userId);

        if (role == Roles.Reader) return Forbid(); // readers can't edit
        if (role == Roles.Editor && note.UserId != userId) return Forbid();

        note.Title = dto.Title;
        note.Content = dto.Content;
        await _notes.UpdateAsync(note);
        return Ok(note);
    }

    // Delete note - only Admin
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var note = await _notes.GetByIdAsync(id);
        if (note == null) return NotFound();
        await _notes.DeleteAsync(id);
        return NoContent();
    }
}
