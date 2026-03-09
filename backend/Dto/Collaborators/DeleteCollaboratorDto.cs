using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Collaborators;

public class DeleteCollaboratorDto
{
    [Required] public required Guid NoteId { get; set; }

    [Required] public required Guid UserId { get; set; }
}