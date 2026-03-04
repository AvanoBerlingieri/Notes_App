using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Collaborators;

public class CollaboratorResponse
{
    [Required] public required Guid UserId { get; set; }
    
    [Required] public required string Email { get; set; }
    
    [Required] public required string Role { get; set; }
}