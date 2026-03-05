using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.DTO.Notes;

namespace NotesApp.Hub;

/// <summary>
/// Hub for live collaborative editing of notes
/// Each note has a "room" identified by its NoteId
/// </summary>
[Authorize]
public class NoteHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly ApplicationDbContext _context;

    public NoteHub(ApplicationDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// User joins a note "room" to receive real-time updates
    /// </summary>
    public async Task JoinNoteRoom(Guid noteId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, noteId.ToString());
    }

    /// <summary>
    /// User leaves a note "room"
    /// </summary>
    public async Task LeaveNoteRoom(Guid noteId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, noteId.ToString());
    }

    /// <summary>
    /// Called when a user edits a note. Broadcasts to all clients in the note room except sender
    /// </summary>
    /// <param name="noteId">Id of the note being edited</param>
    /// <param name="content">Updated note content</param>
    public async Task EditNote(Guid noteId, string content)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(n => n.NoteId == noteId);

        if (note == null)
            return;

        // Save change to database
        note.Content = content;
        note.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Broadcast update to everyone else
        await Clients.OthersInGroup(noteId.ToString())
            .SendAsync("ReceiveNoteUpdate", noteId, content);
    }
}