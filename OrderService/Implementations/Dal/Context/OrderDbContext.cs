using Dal.Abstractions.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Dal.Context;

public sealed class OrderDbContext(
    DbContextOptions<OrderDbContext> options)
    : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<ArchiveFile> ArchiveFiles => Set<ArchiveFile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("order");
        
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        
        var order = modelBuilder.Entity<Order>();

        order.HasKey(x => x.Id);

        order.Property(x => x.Id)
            .IsRequired();
        
        order.Property(x => x.UserId)
            .IsRequired();
        
        order.Property(x => x.CameraId)
            .IsRequired();
        
        order.Property(x => x.FromUtc)
            .IsRequired();
        
        order.Property(x => x.ToUtc)
            .IsRequired();
        
        order.Property(x => x.Status)
            .IsRequired();

        order.Property(x => x.FailureReason);
        
        order.Property(x => x.CreatedAtUtc)
            .IsRequired();
        
        order.Property(x => x.UpdatedAtUtc)
            .IsRequired();
        
        modelBuilder.Entity<ArchiveFile>(b =>
        {
            b.ToTable("ArchiveFiles", "order");

            b.HasKey(x => x.Id);

            b.Property(x => x.OriginalFileName)
                .IsRequired()
                .HasMaxLength(260);

            b.Property(x => x.StoredFileName)
                .IsRequired()
                .HasMaxLength(260);

            b.Property(x => x.StoragePath)
                .IsRequired()
                .HasMaxLength(1024);

            b.Property(x => x.ContentType)
                .IsRequired()
                .HasMaxLength(128);

            b.Property(x => x.FileSize)
                .IsRequired();

            b.Property(x => x.CreatedAtUtc)
                .IsRequired();

            b.HasOne(x => x.Order)
                .WithOne(x => x.ArchiveFile)
                .HasForeignKey<ArchiveFile>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.OrderId).IsUnique();
        });
    }
}