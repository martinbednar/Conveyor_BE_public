using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class DevicePartConfiguration : IEntityTypeConfiguration<DevicePart>
    {
        public void Configure(EntityTypeBuilder<DevicePart> modelBuilder)
        {
            modelBuilder.HasIndex(dp => dp.Name)
                .IsUnique();
            modelBuilder.HasIndex(dp => dp.NodeId)
                .IsUnique();


            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}