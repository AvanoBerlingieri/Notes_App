using System.ComponentModel.DataAnnotations;
using NotesApp.Model;

namespace NotesApp.DTO.Collaborators;

public class CollaboratorResponseDto
{
    [Required] public required Guid UserId { get; set; }

    [Required] public required string UserName { get; set; }

    [Required] public required NoteRole Role { get; set; }
}