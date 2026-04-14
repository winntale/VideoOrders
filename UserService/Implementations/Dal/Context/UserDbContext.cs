using Dal.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dal.Context;

public sealed class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }
    
    public const string ConnectionDatabase = "Users";

    public DbSet<User> Users => Set<User>();

    public DbSet<UserCameraAccess> UserCameraAccesses => Set<UserCameraAccess>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("user_access");
        
        var user = modelBuilder.Entity<User>();
        user.HasKey(x => x.Id);
        user.Property(x => x.Login).IsRequired().HasMaxLength(128);
        user.Property(x => x.Status).IsRequired();

        var access = modelBuilder.Entity<UserCameraAccess>();
        access.HasKey(x => x.Id);
        access.Property(x => x.UserId).IsRequired();
        access.Property(x => x.CameraId).IsRequired();
        access.HasIndex(x => new { x.UserId, x.CameraId }).IsUnique();
    }
}