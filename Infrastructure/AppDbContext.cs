using DomainEntity.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<Storage> Storages => Set<Storage>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<Measurement> Measurements => Set<Measurement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.HasMany(p => p.Batches)
                  .WithOne(b => b.Product)
                  .HasForeignKey(b => b.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Batch>(entity =>
        {
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Number)
                  .IsRequired();

            entity.HasOne(b => b.Product)
                  .WithMany(s => s.Batches)
                  .HasForeignKey(b => b.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.Storage)
                  .WithMany(s => s.Batches)
                  .HasForeignKey(b => b.StorageId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(s => s.Address)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.HasMany(s => s.Batches)
                  .WithOne(b => b.Storage)
                  .HasForeignKey(b => b.StorageId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(s => s.Sensors)
                  .WithOne(d => d.Storage)
                  .HasForeignKey(d => d.StorageId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Surname).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Phone).IsRequired().HasMaxLength(20);
            entity.Property(u => u.Role).IsRequired().HasMaxLength(30);
        });

        modelBuilder.Entity<User>()
            .HasOne(c => c.Storage)
            .WithMany(s => s.Users)
            .HasForeignKey(c => c.StorageId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Type)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.HasOne(s => s.Storage)
                  .WithMany(st => st.Sensors)
                  .HasForeignKey(s => s.StorageId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.Measurements)
                  .WithOne(m => m.Sensor)
                  .HasForeignKey(m => m.SensorId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Timestamp)
                  .IsRequired();
        });
    }
}
