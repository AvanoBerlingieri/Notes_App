using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Auth;

public class AuthResponseDto
{
    [Required] public required string Token { get; set; }

    [Required] public required DateTime Expiration { get; set; }

    [Required] public required Guid UserId { get; set; }

    [Required] public required string Email { get; set; }

    [Required] public required string UserName { get; set; }
    
    [MaxLength(25)] public string? FirstName { get; set; }

    [MaxLength(25)] public string? LastName { get; set; }
}