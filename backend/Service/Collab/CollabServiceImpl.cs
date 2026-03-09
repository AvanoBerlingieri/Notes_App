using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.DTO.Collaborators;
using NotesApp.Model;

namespace NotesApp.Service.Collab;

public class CollabServiceImpl : ICollabService
{
    private readonly ApplicationDbContext _context;

    public CollabServiceImpl(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Creates a collaboration between user and note
    /// </summary>
    /// <param name="dto">DTO containing collaborator data</param>
    /// <param name="noteOwnerId">ID of the note owner</param>
    /// <returns>Collaborator response dto if collaborator is created</returns>
    /// <exception cref="ArgumentException">thrown if userId or noteId are empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">thrown if role is outside or NoteRole enum range</exception>
    /// <exception cref="InvalidOperationException">
    ///     thrown when owner trys to add their self as collaborator or if adding an
    ///     already existing collaborator
    /// </exception>
    /// <exception cref="KeyNotFoundException">thrown if note or user can't be found</exception>
    /// <exception cref="UnauthorizedAccessException">thrown if someone other than the note owner trys to add collaborator</exception>
    public async Task<CollaboratorResponseDto> AddCollaborator(CreateCollaboratorDto dto, Guid noteOwnerId)
    {
        if (dto.UserId == Guid.Empty || dto.NoteId == Guid.Empty)
            throw new ArgumentException("UserId and NoteId are required.");

        if (!Enum.IsDefined(typeof(NoteRole), dto.Role)) throw new ArgumentOutOfRangeException(nameof(dto.Role));

        if (dto.UserId == noteOwnerId) throw new InvalidOperationException("Owner cannot be added as collaborator.");

        var note = await _context.Notes.FirstOrDefaultAsync(n => n.NoteId == dto.NoteId);
        if (note == null) throw new KeyNotFoundException("Note not found.");

        if (note.OwnerId != noteOwnerId) throw new UnauthorizedAccessException("Only the owner can add collaborators.");

        var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists) throw new KeyNotFoundException("User not found.");

        var exists = await _context.NoteCollaborators
            .AnyAsync(c => c.NoteId == dto.NoteId && c.UserId == dto.UserId);

        if (exists) throw new InvalidOperationException("User is already a collaborator.");

        var collab = new NoteCollaborator
        {
            NoteId = dto.NoteId,
            UserId = dto.UserId,
            Role = dto.Role
        };

        await _context.NoteCollaborators.AddAsync(collab);
        await _context.SaveChangesAsync();

        return new CollaboratorResponseDto
        {
            NoteId = collab.NoteId,
            UserId = collab.UserId,
            Role = collab.Role
        };
    }

    /// <summary>
    ///     Deletes a collaboration from a note. Only the note owner is allowed to perform this operation.
    /// </summary>
    /// <param name="dto">DTO containing collaborator data</param>
    /// <param name="noteOwnerId">ID of the note owner</param>
    /// <exception cref="ArgumentException">thrown if userId or noteId are empty</exception>
    /// <exception cref="InvalidOperationException">thrown if owner tries to remove itself</exception>
    /// <exception cref="KeyNotFoundException">thrown if collaboration not found</exception>
    /// <exception cref="UnauthorizedAccessException">thrown if user trying to use this method is not the owner</exception>
    public async Task RemoveCollaborator(DeleteCollaboratorDto dto, Guid noteOwnerId)
    {
        if (dto.UserId == Guid.Empty || dto.NoteId == Guid.Empty)
            throw new ArgumentException("UserId and NoteId are required.");

        if (dto.UserId == noteOwnerId)
            throw new InvalidOperationException("Owner cannot remove itself.");

        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.NoteId == dto.NoteId);

        if (note == null)
            throw new KeyNotFoundException("Note not found.");

        if (note.OwnerId != noteOwnerId)
            throw new UnauthorizedAccessException("Only the owner can remove collaborators.");

        var collab = await _context.NoteCollaborators
            .FirstOrDefaultAsync(c => c.NoteId == dto.NoteId && c.UserId == dto.UserId);

        if (collab == null)
            throw new KeyNotFoundException("Collaboration not found.");

        _context.NoteCollaborators.Remove(collab);
        await _context.SaveChangesAsync();
    }

    public async Task<CollaboratorResponseDto> UpdateRole(UpdateRoleDto dto, Guid collabId, Guid noteOwnerId)
    {
        throw new NotImplementedException();
    }
}