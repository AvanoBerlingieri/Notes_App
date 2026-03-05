using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Notes;

public class CreateNoteDto
{
    [Required] [MaxLength(50)] public required string Title { get; set; }

    [Required] public required string Content { get; set; } = " ";
}