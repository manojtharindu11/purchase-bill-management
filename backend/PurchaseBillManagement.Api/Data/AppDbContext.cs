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
        }

    }
}
