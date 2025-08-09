using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Shared.Models;
using WarehouseManagement.Shared.Enums;

namespace WarehouseManagement.Server.Data
{
    public class WarehouseContext : DbContext
    {
        public WarehouseContext(DbContextOptions<WarehouseContext> options) : base(options)
        {
        }

        public DbSet<Resource> Resources { get; set; }
        public DbSet<UnitOfMeasurement> UnitsOfMeasurement { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<ReceiptDocument> ReceiptDocuments { get; set; }
        public DbSet<ReceiptResource> ReceiptResources { get; set; }
        public DbSet<ShipmentDocument> ShipmentDocuments { get; set; }
        public DbSet<ShipmentResource> ShipmentResources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка таблицы Resource
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.State).HasConversion<int>();
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Настройка таблицы UnitOfMeasurement
            modelBuilder.Entity<UnitOfMeasurement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.State).HasConversion<int>();
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Настройка таблицы Client
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.State).HasConversion<int>();
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Настройка таблицы Balance
            modelBuilder.Entity<Balance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.Resource)
                    .WithMany()
                    .HasForeignKey(e => e.ResourceId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.UnitOfMeasurement)
                    .WithMany()
                    .HasForeignKey(e => e.UnitOfMeasurementId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.ResourceId, e.UnitOfMeasurementId }).IsUnique();
            });

            // Настройка таблицы ReceiptDocument
            modelBuilder.Entity<ReceiptDocument>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Number).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Number).IsUnique();
            });

            // Настройка таблицы ReceiptResource
            modelBuilder.Entity<ReceiptResource>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.ReceiptDocument)
                    .WithMany(d => d.ReceiptResources)
                    .HasForeignKey(e => e.ReceiptDocumentId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Resource)
                    .WithMany()
                    .HasForeignKey(e => e.ResourceId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.UnitOfMeasurement)
                    .WithMany()
                    .HasForeignKey(e => e.UnitOfMeasurementId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Настройка таблицы ShipmentDocument
            modelBuilder.Entity<ShipmentDocument>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Number).IsRequired().HasMaxLength(50);
                entity.Property(e => e.State).HasConversion<int>();
                entity.HasIndex(e => e.Number).IsUnique();
                
                entity.HasOne(e => e.Client)
                    .WithMany()
                    .HasForeignKey(e => e.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Настройка таблицы ShipmentResource
            modelBuilder.Entity<ShipmentResource>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.ShipmentDocument)
                    .WithMany(d => d.ShipmentResources)
                    .HasForeignKey(e => e.ShipmentDocumentId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Resource)
                    .WithMany()
                    .HasForeignKey(e => e.ResourceId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.UnitOfMeasurement)
                    .WithMany()
                    .HasForeignKey(e => e.UnitOfMeasurementId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
