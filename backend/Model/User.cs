using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace NotesApp.Model;

public class User : IdentityUser<Guid>
{
    /* This is handled by identity
    Guid Id { get; set; }
    string UserName { get; set; }
    Email { get; set; }    
    string PasswordHash { get; set; }
    */
    
    [MaxLength(25)] public string? FirstName { get; set; }
   
    [MaxLength(25)] public string? LastName { get; set; }
   
    public DateTime DateCreated { get; private set; } = DateTime.UtcNow;
    
    // nav properties
    public ICollection<Note> OwnedNotes { get; set; } = new List<Note>();
    public ICollection<NoteCollaborator> Collaborations { get; set; } = new List<NoteCollaborator>();
}