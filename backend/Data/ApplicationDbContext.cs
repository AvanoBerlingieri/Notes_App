using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotesApp.Model;

namespace NotesApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Note> Notes { get; set; }

    public DbSet<NoteCollaborator> NoteCollaborators { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Composite key
        builder.Entity<NoteCollaborator>()
            .HasKey(nc => new { nc.NoteId, nc.UserId });

        builder.Entity<Note>()
            .HasOne(n => n.Owner)
            .WithMany(u => u.OwnedNotes)
            .HasForeignKey(n => n.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<NoteCollaborator>()
            .HasOne(nc => nc.User)
            .WithMany(u => u.Collaborations)
            .HasForeignKey(nc => nc.UserId);

        builder.Entity<NoteCollaborator>()
            .HasOne(nc => nc.Note)
            .WithMany(n => n.Collaborators)
            .HasForeignKey(nc => nc.NoteId);
    }
}