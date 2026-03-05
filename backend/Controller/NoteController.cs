using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApp.DTO.Notes;
using NotesApp.Service.Notes;
namespace NotesApp.Controller;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }
    
    /// <summary>
    /// Get a single note
    /// </summary>
    [HttpGet("{noteId:guid}")]
    public async Task<IActionResult> GetNote(Guid noteId, [FromQuery] Guid userId)
    {
        try
        {
            var note = await _noteService.GetNoteAsync(userId, noteId);
            return Ok(note);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all notes accessible to the user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllNotes([FromQuery] Guid userId)
    {
        var notes = await _noteService.GetAllNotesAsync(userId);
        return Ok(notes);
    }
    
    // Create note
    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteDto dto, [FromQuery] Guid userId)
    {
        var note = await _noteService.CreateNoteAsync(userId, dto);
        return StatusCode(201, note);
    }

    // Delete note
    [HttpDelete("{noteId:guid}")]
    public async Task<IActionResult> DeleteNote(Guid noteId, [FromQuery] Guid userId)
    {
        try {
            await _noteService.DeleteNoteAsync(userId, noteId);
            return Ok(new { message = "Note deleted successfully." });
        } catch (Exception ex) {
            return BadRequest(new { message = ex.Message });
        }
    }

    // edit content of note
    [HttpPut("{noteId:guid}")]
    public async Task<IActionResult> EditNote(Guid noteId, [FromBody] UpdateNoteDto dto, [FromQuery] Guid userId)
    {
        try {
            var note = await _noteService.EditNoteAsync(userId, noteId, dto.Content);
            return Ok(note);
        } catch (Exception ex) {
            return BadRequest(new { message = ex.Message });
        }
    }
}