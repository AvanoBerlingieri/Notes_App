using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotesApp.Model;

namespace NotesApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Note> Notes { get; set; }
    
    public DbSet<NoteCollaborator> NoteCollaborators { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { }