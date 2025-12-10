using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Data;
public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    // Users Management
    public DbSet<User> Users { get; set; }
    public DbSet<SprintItem> SprintItems { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Project> Projects { get; set; }
    
    public DbSet<UserAssignedProject> UserAssignedProjects { get; set; }
    public DbSet<UserSprintItem> UserSprintItems { get; set; }
    public DbSet<UserFeature> UserFeatures { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserAssignedProject>()
            .HasOne(uap => uap.User)
            .WithMany(u => u.UserAssignedProjects)
            .HasForeignKey(u => u.UserID)
            .OnDelete(DeleteBehavior.NoAction);  // Avoid cascade delete

        // Project to UserAssignedProjects
        modelBuilder.Entity<UserAssignedProject>()
            .HasOne(uap => uap.Project)
            .WithMany(p => p.UserAssignedProjects)
            .HasForeignKey(uap => uap.ProjectID)
            .OnDelete(DeleteBehavior.NoAction);

        // User to CreatedProjects
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Creator)
            .WithMany(u => u.CreatedProjects)
            .HasForeignKey(p => p.CreatorID)
            .OnDelete(DeleteBehavior.NoAction);  // Avoid cascade delete
        
    }

    
}
