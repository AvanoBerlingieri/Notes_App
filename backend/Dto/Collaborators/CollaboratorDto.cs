using System.ComponentModel.DataAnnotations;
using NotesApp.Model;

namespace NotesApp.DTO.Collaborators;

public class CollaboratorDto
{
    [Required] public required Guid NoteId { get; set; }

    [Required] public required string UserName { get; set; }

    [Required] public required NoteRole Role { get; set; }
}