using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Notes;

public class UpdateNoteTitleDto
{
    [Required] public required string Title { get; set; }
}