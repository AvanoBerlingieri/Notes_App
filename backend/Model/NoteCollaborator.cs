using System.ComponentModel.DataAnnotations;
namespace NotesApp.Model;

// This is a join table for notes and users
public class NoteCollaborator
{
    [Required] public Guid NoteId { get; set; }

    [Required] public Guid UserId { get; set; }

    [Required] public required NoteRole Role { get; set; }  

    public DateTime DateAdded { get; private set; } = DateTime.UtcNow;

    // nav properties
    public Note Note { get; set; } = null!;
    public User User { get; set; } = null!;
}