using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApp.DTO.Collaborators;
using NotesApp.Service.Collab;

namespace NotesApp.Controller;

[ApiController]
[Route("api/[controller]")]
public class CollabController : ControllerBase
{
    private readonly ICollabService _collabService;

    public CollabController(ICollabService collabService)
    {
        _collabService = collabService;
    }

    /// <summary>
    ///     Retrieve list of collaborators for a note
    /// </summary>
    /// <param name="noteId"></param>
    /// <returns>
    ///     Returns 200 code with list of collaborators
    ///     Returns 400 code if DTO IDs are missing,
    ///     Returns 401 code if user is not authorized by jwt
    ///     Returns 403 code if user is not the owner or a collaborator
    ///     Returns 404 if note was not found
    /// </returns>
    [Authorize]
    [HttpGet("{noteId:Guid}/collaborators")]
    public async Task<IActionResult> GetAllCollaborators([FromRoute] Guid noteId)
    {
        // Grab userId from the claims in the JWT
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var collabs = await _collabService.GetCollaborators(noteId, userId);
            return Ok(collabs);
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(403, new { message = e.Message });
        }
    }

    /// <summary>
    ///     Create collaborator API
    /// </summary>
    /// <param name="dto">DTO containing collaborator info</param>
    /// <returns>
    ///     Returns 200 code with message and collaborator object
    ///     Returns 400 code if DTO IDs are missing, DTO Role is not in NoteRole,
    ///     or owner tries to add themselves as a collaborator
    ///     Returns 401 code if user is not authorized by jwt
    ///     Returns 403 code if user attempting to add collaborator is not the owner
    ///     Returns 404 if note or user were not found
    /// </returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCollaborator([FromBody] CreateCollaboratorDto dto)
    {
        // Grab userId from the claims in the JWT
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var collab = await _collabService.AddCollaborator(dto, userId);
            return Ok(new
            {
                message = "Collaborator created successfully",
                collaborator = collab
            });
        }
        catch (ArgumentOutOfRangeException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(403, new { message = e.Message });
        }
    }

    /// <summary>
    ///     Delete collaborator API
    /// </summary>
    /// <param name="dto">DTO containing collaborator info</param>
    /// <returns>
    ///     Returns 200 code with message
    ///     Returns 400 code if DTO IDs are missing, or owner tries to remove themselves
    ///     Returns 401 code if user is not authorized by jwt
    ///     Returns 403 code if user attempting to add collaborator is not the owner
    ///     Returns 404 if note or collaborator record were not found
    /// </returns>
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteCollaborator([FromBody] DeleteCollaboratorDto dto)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            await _collabService.RemoveCollaborator(dto, userId);
            return NoContent();
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { e.Message });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new { e.Message });
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { e.Message });
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(403, new { message = e.Message });
        }
    }

    /// <summary>
    ///     Update collaborator API
    /// </summary>
    /// <param name="dto">DTO containing collaborator info</param>
    /// <returns>
    ///     Returns 200 code with message and updated collaborator object
    ///     Returns 400 code if DTO IDs are missing, DTO Role is not in NoteRole,
    ///     or owner tries to change their own role
    ///     Returns 401 code if user is not authorized by jwt
    ///     Returns 403 code if user attempting to update collaboration is not the owner
    ///     Returns 404 if note or collaboration were not found
    /// </returns>
    [Authorize]
    [HttpPatch("role")]
    public async Task<IActionResult> UpdateCollaborator([FromBody] UpdateRoleDto dto)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var collab = await _collabService.UpdateRole(dto, userId);
            return Ok(new
            {
                message = "Collaborator created successfully!",
                collaborator = collab
            });
        }
        catch (ArgumentOutOfRangeException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(403, new { message = e.Message });
        }
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}