using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Auth;

public class LoginDto
{
    [Required] public required string UserName { get; set; }
    
    [Required] public required string Password { get; set; }
}