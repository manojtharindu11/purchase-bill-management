using Microsoft.EntityFrameworkCore;
using PurchaseBillManagement.Api.Models;

namespace PurchaseBillManagement.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<LocationDetail> LocationDetails => Set<LocationDetail>();
        public DbSet<PurchaseBill> PurchaseBills => Set<PurchaseBill>();
        public DbSet<PurchaseBillItem> PurchaseBillItems => Set<PurchaseBillItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LocationDetail>(entity =>
            {
                entity.ToTable("Location_Details");
                entity.HasKey(e => e.LocationCode);
                entity.Property(e => e.LocationCode)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasColumnName("Location_Code");
                entity.Property(e => e.LocationName)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnName("Location_Name");
            });

            modelBuilder.Entity<PurchaseBill>(entity =>
            {
                entity.ToTable("Purchase_Bills");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BillNumber).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.BillNumber).IsUnique();
                entity.Property(e => e.TotalQuantity).HasPrecision(18, 3);
                entity.Property(e => e.TotalCost).HasPrecision(18, 2);
                entity.Property(e => e.TotalSelling).HasPrecision(18, 2);

                entity.HasMany(e => e.Items)
                      .WithOne(i => i.PurchaseBill)
                      .HasForeignKey(i => i.PurchaseBillId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PurchaseBillItem>(entity =>
            {
                entity.ToTable("Purchase_Bill_Items");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ItemName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.BatchLocationName).HasMaxLength(200);
                entity.Property(e => e.StandardCost).HasPrecision(18, 2);
                entity.Property(e => e.StandardPrice).HasPrecision(18, 2);
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
                entity.Property(e => e.DiscountPercent).HasPrecision(5, 2);
                entity.Property(e => e.TotalCost).HasPrecision(18, 2);
                entity.Property(e => e.TotalSelling).HasPrecision(18, 2);
            });
        }
    }
}
