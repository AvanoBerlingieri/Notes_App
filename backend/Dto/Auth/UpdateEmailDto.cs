using System.ComponentModel.DataAnnotations;

namespace NotesApp.DTO.Auth;

public class UpdateEmailDto
{
    [EmailAddress] public required string? CurrentEmail { get; set; }
    
    [EmailAddress] public required string? NewEmail { get; set; }
}