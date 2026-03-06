using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.DTO.Notes;
using NotesApp.Model;

namespace NotesApp.Service.Notes;

public class NoteService : INoteService
{
    private readonly ApplicationDbContext _context;

    public NoteService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Creates a new note and assigns the current user as the Owner.
    /// </summary>
    /// <param name="userId">The ID of the user creating the note.</param>
    /// <param name="dto">The title and initial content.</param>
    public async Task<NoteResponseDto> CreateNoteAsync(Guid userId, CreateNoteDto dto)
    {
        var note = new Note
        {
            Title = dto.Title,
            Content = dto.Content,
            OwnerId = userId
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        return new NoteResponseDto
        {
            NoteId = note.NoteId,
            Title = note.Title,
            Content = note.Content,
            OwnerId = note.OwnerId,
            DateCreated = note.DateCreated,
            LastModified = note.LastModified,
            CurrentUserRole = NoteRole.Owner
        };
    }

    /// <summary>
    ///     Deletes a note from the database.
    ///     Only the note owner is allowed to perform this operation.
    /// </summary>
    /// <param name="userId">The ID of the user attempting the deletion.</param>
    /// <param name="noteId">The ID of the note to delete.</param>
    /// <exception cref="Exception">
    ///     Thrown if the note does not exist or if the user is not the owner.
    /// </exception>
    public async Task DeleteNoteAsync(Guid userId, Guid noteId)
    {
        var note = await _context.Notes
            .Include(n => n.Collaborators)
            .FirstOrDefaultAsync(n => n.NoteId == noteId);

        if (note == null) throw new Exception("Note not found.");

        if (note.OwnerId != userId) throw new Exception("Only the owner can delete this note.");

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    ///     Updates the content of an existing note.
    ///     The user must either be the owner or have the Editor role as a collaborator.
    /// </summary>
    /// <param name="userId">The ID of the user attempting to edit the note.</param>
    /// <param name="noteId">The ID of the note being edited.</param>
    /// <param name="newContent">The updated content of the note.</param>
    /// <returns>
    ///     A DTO containing the updated note content.
    /// </returns>
    /// <exception cref="Exception">
    ///     Thrown if the note does not exist or the user does not have edit permission.
    /// </exception>
    public async Task<UpdateNoteDto> EditNoteAsync(Guid userId, Guid noteId, string newContent)
    {
        var note = await _context.Notes
            .Include(n => n.Collaborators)
            .FirstOrDefaultAsync(n => n.NoteId == noteId);

        if (note == null) throw new Exception("Note not found.");

        // Check if user has editing rights
        var canEdit = note.OwnerId == userId ||
                      note.Collaborators.Any(c => c.UserId == userId && c.Role == NoteRole.Editor);

        if (!canEdit) throw new Exception("You do not have permission to edit this note.");

        note.Content = newContent;
        note.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Return updated note
        return new UpdateNoteDto
        {
            Content = note.Content
        };
    }

    /// <summary>
    ///     Retrieves a specific note if the user has permission to view it.
    ///     Determines the user's role (Owner, Editor, Viewer) relative to the note.
    /// </summary>
    /// <param name="userId">The ID of the requesting user.</param>
    /// <param name="noteId">The ID of the note to retrieve.</param>
    /// <returns>
    ///     A DTO containing note details and the current user's role.
    /// </returns>
    /// <exception cref="Exception">
    ///     Thrown if the note does not exist.
    /// </exception>
    public async Task<NoteResponseDto> GetNoteAsync(Guid userId, Guid noteId)
    {
        var note = await _context.Notes
            .Include(n => n.Collaborators)
            .FirstOrDefaultAsync(n => n.NoteId == noteId);

        if (note == null)
            throw new Exception("Note not found.");

        var role = note.OwnerId == userId
            ? NoteRole.Owner
            : note.Collaborators.FirstOrDefault(c => c.UserId == userId)?.Role ?? NoteRole.Viewer;

        return new NoteResponseDto
        {
            NoteId = note.NoteId,
            Title = note.Title,
            Content = note.Content,
            OwnerId = note.OwnerId,
            DateCreated = note.DateCreated,
            LastModified = note.LastModified,
            CurrentUserRole = role
        };
    }

    /// <summary>
    ///     Retrieves all notes accessible to the user.
    ///     Includes notes owned by the user and notes shared with the user as a collaborator.
    /// </summary>
    /// <param name="userId">The ID of the requesting user.</param>
    /// <returns>
    ///     A list of note DTOs containing note metadata and the user's role for each note.
    /// </returns>
    public async Task<List<NoteResponseDto>> GetAllNotesAsync(Guid userId)
    {
        return await _context.Notes
            .Where(n => n.OwnerId == userId || n.Collaborators
                .Any(c => c.UserId == userId))
            .Select(n => new NoteResponseDto
            {
                NoteId = n.NoteId,
                Title = n.Title,
                Content = n.Content,
                OwnerId = n.OwnerId,
                DateCreated = n.DateCreated,
                LastModified = n.LastModified,
                CurrentUserRole = n.OwnerId == userId
                    ? NoteRole.Owner
                    : n.Collaborators.First(c => c.UserId == userId).Role
            }).ToListAsync();
    }
}