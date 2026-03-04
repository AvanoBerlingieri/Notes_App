using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NotesApp.Model;

public class Note
{
    [Key]
    public Guid NoteId { get; set; } = Guid.NewGuid();

    [Required] [MaxLength(50)] public required string Title { get; set; }

    [Required] public required string Content { get; set; }

    public DateTime DateCreated { get; private set; } = DateTime.UtcNow;

    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    // Owner Relationship
    [Required] public Guid OwnerId { get; set; }
    [ForeignKey(nameof(OwnerId))] public User Owner { get; set; } = null!;

    // nav properties
    public ICollection<NoteCollaborator> Collaborators { get; set; } = new List<NoteCollaborator>();
}