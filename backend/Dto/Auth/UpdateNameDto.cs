using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Auth;

public class UpdateNameDto
{
    [MaxLength(25)] public string? FirstName { get; set; }

    [MaxLength(25)] public string? LastName { get; set; }
}