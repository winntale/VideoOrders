using Dal.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Dal.Context;

public sealed class VideoArchiveDbContext : DbContext
{
    public VideoArchiveDbContext(DbContextOptions<VideoArchiveDbContext> options)
        : base(options)
    {
    }

    public DbSet<Camera> Cameras => Set<Camera>();
    public DbSet<VideoSegment> VideoSegments => Set<VideoSegment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("video_archive");

        ConfigureCameras(modelBuilder);
        ConfigureVideoSegments(modelBuilder);
    }
    
    private static void ConfigureCameras(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Camera>();

        entity.ToTable("Cameras");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .IsRequired();

        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(256);

        entity.Property(x => x.IsActive)
            .IsRequired();

        entity.HasMany(x => x.VideoSegments)
            .WithOne(x => x.Camera)
            .HasForeignKey(x => x.CameraId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureVideoSegments(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<VideoSegment>();

        entity.ToTable("VideoSegments");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .IsRequired();

        entity.Property(x => x.CameraId)
            .IsRequired();

        entity.Property(x => x.FromUtc)
            .IsRequired();

        entity.Property(x => x.ToUtc)
            .IsRequired();

        entity.HasIndex(x => new { x.CameraId, x.FromUtc, x.ToUtc });
    }
}