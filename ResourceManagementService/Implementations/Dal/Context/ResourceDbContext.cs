using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dal.Context;

public sealed class ResourceDbContext : DbContext
{
    public ResourceDbContext(DbContextOptions<ResourceDbContext> options)
        : base(options)
    {
    }

    public const string ConnectionDatabase = "Resources";

    public DbSet<Resource> Resources => Set<Resource>();

    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("resources");

        var resource = modelBuilder.Entity<Resource>();
        resource.ToTable("Resources");
        resource.HasKey(x => x.Id);
        resource.Property(x => x.Type).IsRequired();
        resource.Property(x => x.TotalCapacity).IsRequired();
        resource.Property(x => x.ReservedAmount).IsRequired();
        resource.Property(x => x.Unit).IsRequired().HasMaxLength(16);
        resource.HasIndex(x => x.Type).IsUnique();

        var reservation = modelBuilder.Entity<Reservation>();
        reservation.ToTable("Reservations");
        reservation.HasKey(x => x.Id);
        reservation.Property(x => x.OrderId).IsRequired();
        reservation.Property(x => x.ResourceType).IsRequired();
        reservation.Property(x => x.Amount).IsRequired();
        reservation.Property(x => x.ReservedAtUtc).IsRequired();
        reservation.Property(x => x.HoldUntilUtc).IsRequired();
        reservation.Property(x => x.Status).IsRequired();
        reservation.HasIndex(x => x.OrderId);
        reservation.HasIndex(x => new { x.ResourceType, x.Status });

        SeedResources(resource);
    }

    private static void SeedResources(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Resource> resource)
    {
        resource.HasData(
            new Resource { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Type = ResourceType.Cpu, TotalCapacity = 32, ReservedAmount = 0, Unit = "cores" },
            new Resource { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Type = ResourceType.Ram, TotalCapacity = 65_536, ReservedAmount = 0, Unit = "MB" },
            new Resource { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Type = ResourceType.Disk, TotalCapacity = 1_048_576, ReservedAmount = 0, Unit = "MB" });
    }
}
