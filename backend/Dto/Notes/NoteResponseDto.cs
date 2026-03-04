using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Notes;

public class NoteResponseDto
{
    [Required] public required Guid NoteId { get; set; }
   
    [Required] public required string Title { get; set; }
   
    [Required] public required string Content { get; set; }
   
    [Required] public required Guid OwnerId { get; set; }
   
    [Required] public required DateTime DateCreated { get; set; }
   
    [Required] public required DateTime LastModified { get; set; }
}