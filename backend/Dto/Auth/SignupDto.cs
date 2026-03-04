using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Auth;

public class SignupDto
{
    [Required] [EmailAddress] public required string Email { get; set; }

    [Required]
    [MinLength(5)]
    [MaxLength(25)]
    public required string UserName { get; set; }

    [Required] [MinLength(8)] public required string Password { get; set; }

    [MaxLength(25)] public string? FirstName { get; set; }

    [MaxLength(25)] public string? LastName { get; set; }
}