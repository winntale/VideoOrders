using Dal.Abstractions.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Dal.Context;

public sealed class OrderDbContext(
    DbContextOptions<OrderDbContext> options)
    : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

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
    }
}