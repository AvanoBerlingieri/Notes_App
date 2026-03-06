using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    ///     Retrieves a single note by its ID if the authenticated user has access to it.
    /// </summary>
    /// <param name="noteId">The unique identifier of the note.</param>
    /// <returns>
    ///     Returns the requested note if the user has permission to view it.
    ///     Returns 401 if the user is not authenticated.
    ///     Returns 404 if the note does not exist or is not accessible.
    /// </returns>
    [Authorize]
    [HttpGet("{noteId:guid}")]
    public async Task<IActionResult> GetNote(Guid noteId)
    {
        // Grab userId from the claims in the JWT
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

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
    ///     Retrieves all notes that belong to the authenticated user.
    ///     Includes notes owned by the user and notes where the user is a collaborator.
    /// </summary>
    /// <returns>
    ///     A list of notes accessible by the authenticated user.
    ///     Returns 401 if the user is not authenticated.
    /// </returns>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllNotes()
    {
        // Grab userId from the claims in the JWT
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var notes = await _noteService.GetAllNotesAsync(userId);
        return Ok(notes);
    }

    /// <summary>
    ///     Creates a new note for the authenticated user.
    ///     The user creating the note automatically becomes the owner.
    /// </summary>
    /// <param name="dto">
    ///     Data required to create the note including the title and initial content.
    /// </param>
    /// <returns>
    ///     Returns the newly created note with metadata such as creation date and owner information.
    ///     Returns 401 if the user is not authenticated.
    /// </returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteDto dto)
    {
        // Grab userId from the claims in the JWT
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var note = await _noteService.CreateNoteAsync(userId, dto);

        return StatusCode(201, note);
    }

    /// <summary>
    ///     Deletes a note owned by the authenticated user.
    ///     Only the note owner is allowed to delete the note.
    /// </summary>
    /// <param name="noteId">The unique identifier of the note to delete.</param>
    /// <returns>
    ///     Returns a success message if the note is deleted.
    ///     Returns 401 if the user is not authenticated.
    ///     Returns 400 if the note does not exist or the user is not the owner.
    /// </returns>
    [Authorize]
    [HttpDelete("{noteId:guid}")]
    public async Task<IActionResult> DeleteNote([FromRoute] Guid noteId)
    {
        // Grab userId from the claims in the JWT
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            await _noteService.DeleteNoteAsync(userId, noteId);
            return Ok(new { message = "Note deleted successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    ///     Updates the content of an existing note.
    ///     Only the owner or collaborators with the Editor role are allowed to edit.
    /// </summary>
    /// <param name="noteId">The unique identifier of the note to edit.</param>
    /// <param name="updatedTitle">Contains the updated note title.</param>
    /// <returns>
    ///     Returns the updated note content if the operation succeeds.
    ///     Returns 401 if the user is not authenticated.
    ///     Returns 400 if the user does not have permission or the note does not exist.
    /// </returns>
    [Authorize]
    [HttpPut("{noteId:guid}")]
    public async Task<IActionResult> EditNote([FromRoute] Guid noteId, [FromBody] string updatedTitle)
    {
        // Grab userId from the claims in the JWT
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var note = await _noteService.EditNoteAsync(userId, noteId, updatedTitle);
            return Ok(note);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    ///     Helper function to grab userId from jwt claims
    /// </summary>
    /// <returns></returns>
    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}