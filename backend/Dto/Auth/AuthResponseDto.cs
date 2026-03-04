using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Auth;

public class AuthResponseDto
{
    [Required] public required string Token { get; set; }
    
    [Required] public required DateTime Expiration { get; set; }

    [Required] public required Guid UserId { get; set; }
   
    [Required] public required string UserName { get; set; }
}