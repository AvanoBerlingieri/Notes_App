using System.ComponentModel.DataAnnotations;
using NotesApp.Model;

namespace NotesApp.DTO.Collaborators;

public class CreateCollaboratorDto
{
    [Required] public required Guid NoteId { get; set; }

    [Required] public required Guid UserId { get; set; }

    [Required] public required NoteRole Role { get; set; }
}