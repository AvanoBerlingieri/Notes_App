using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Auth;

public class UpdateUserDto
{
    [EmailAddress] public required string? Email { get; set; }
    
    [MaxLength(25)] public string? FirstName { get; set; }

    [MaxLength(25)] public string? LastName { get; set; }
}