using Microsoft.AspNetCore.Mvc;
using NotesApp.DTO.Notes;
using NotesApp.Service.Notes;
namespace NotesApp.Controller;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] NoteDto dto)
    {
        var userId = Guid.Parse(User.FindFirst("sub")!.Value);
        var note = await _noteService.CreateNoteAsync(userId, dto);
        return StatusCode(201, note);
    }

    [HttpDelete("{noteId}")]
    public async Task<IActionResult> DeleteNote(Guid noteId)
    {
        var userId = Guid.Parse(User.FindFirst("sub")!.Value);
        await _noteService.DeleteNoteAsync(userId, noteId);
        return StatusCode(200, new {
            message = "Note deleted successfully."
        });
    }

    [HttpPut("{noteId}")]
    public async Task<IActionResult> EditNote(Guid noteId, [FromBody] NoteDto dto)
    {
        var userId = Guid.Parse(User.FindFirst("sub")!.Value);
        var note = await _noteService.EditNoteAsync(userId, noteId, dto.Content);
        return StatusCode(200, note);
    }
}