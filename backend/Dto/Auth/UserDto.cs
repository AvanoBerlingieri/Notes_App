using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Auth;

public class UserDto
{
    [Required] [EmailAddress] public required string Email { get; set; }

    [Required] public required string UserName { get; set; }
    
    public string? FirstName { get; set; } 
    public string? LastName { get; set; }
}