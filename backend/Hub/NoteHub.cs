using Microsoft.AspNetCore.SignalR;
using NotesApp.DTO.Notes;

namespace NotesApp.Hub;

/// <summary>
/// Hub for live collaborative editing of notes
/// Each note has a "room" identified by its NoteId
/// </summary>
public class NoteHub : Microsoft.AspNetCore.SignalR.Hub
{
    /// <summary>
    /// Called when a user edits a note. Broadcasts to all clients in the note room except sender
    /// </summary>
    /// <param name="noteId">Id of the note being edited</param>
    /// <param name="content">Updated note content</param>
    public async Task EditNote(Guid noteId, string content)
    {
        await Clients.OthersInGroup(noteId.ToString())
            .SendAsync("ReceiveNoteUpdate", noteId, content);
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
}