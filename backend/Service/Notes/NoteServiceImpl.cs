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
    /// Creates a new note owned by the user
    /// </summary>
    public async Task<NoteResponseDto> CreateNoteAsync(Guid userId, NoteDto dto)
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
    /// Deletes a note if the user is the owner
    /// </summary>
    public async Task DeleteNoteAsync(Guid userId, Guid noteId)
    {
        var note = await _context.Notes
            .Include(n => n.Collaborators)
            .FirstOrDefaultAsync(n => n.NoteId == noteId);

        if (note == null) {throw new Exception("Note not found.");}

        if (note.OwnerId != userId) {throw new Exception("Only the owner can delete this note.");}

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates the note content and last modified date
    /// </summary>
    public async Task<NoteResponseDto> EditNoteAsync(Guid userId, Guid noteId, string newContent)
    {
        var note = await _context.Notes
            .Include(n => n.Collaborators)
            .FirstOrDefaultAsync(n => n.NoteId == noteId);

        if (note == null) {throw new Exception("Note not found.");}

        // Check if user has editing rights
        var canEdit = note.OwnerId == userId || 
                       note.Collaborators.Any(c => c.UserId == userId && c.Role == NoteRole.Editor);

        if (!canEdit) {throw new Exception("You do not have permission to edit this note.");}

        note.Content = newContent;
        note.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Return updated note
        return new NoteResponseDto
        {
            NoteId = note.NoteId,
            Title = note.Title,
            Content = note.Content,
            OwnerId = note.OwnerId,
            DateCreated = note.DateCreated,
            LastModified = note.LastModified,
            CurrentUserRole = NoteRole.Editor
        };
    }

    /// <summary>
    /// Get a single note
    /// </summary>
    public async Task<NoteResponseDto> GetNoteAsync(Guid userId, Guid noteId)
    {
        var note = await _context.Notes
            .Include(n => n.Collaborators)
            .FirstOrDefaultAsync(n => n.NoteId == noteId);

        if (note == null)
            throw new Exception("Note not found.");

        var role = note.OwnerId == userId ? NoteRole.Owner :
                   note.Collaborators.FirstOrDefault(c => c.UserId == userId)?.Role ?? NoteRole.Viewer;

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
    /// Get all notes the user has access to (owned + collaborations)
    /// </summary>
    public async Task<List<NoteResponseDto>> GetAllNotesAsync(Guid userId)
    {
        var ownedNotes = await _context.Notes
            .Where(n => n.OwnerId == userId)
            .ToListAsync();

        var collabNotes = await _context.NoteCollaborators
            .Where(c => c.UserId == userId)
            .Select(c => c.Note)
            .ToListAsync();

        var allNotes = ownedNotes.Concat(collabNotes).Distinct();

        return allNotes.Select(note => new NoteResponseDto
        {
            NoteId = note.NoteId,
            Title = note.Title,
            Content = note.Content,
            OwnerId = note.OwnerId,
            DateCreated = note.DateCreated,
            LastModified = note.LastModified,
            CurrentUserRole = note.OwnerId == userId ? NoteRole.Owner :
                              note.Collaborators.FirstOrDefault(c => c.UserId == userId)?.Role ?? NoteRole.Viewer
        }).ToList();
    }
}