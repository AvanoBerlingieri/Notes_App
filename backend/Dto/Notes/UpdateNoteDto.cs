using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Notes;

public class UpdateNoteDto
{
    [Required] public required string Content { get; set; }
}