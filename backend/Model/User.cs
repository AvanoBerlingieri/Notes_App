using System.ComponentModel.DataAnnotations;
namespace NotesApp.Model;

using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<Guid>
{
  
}