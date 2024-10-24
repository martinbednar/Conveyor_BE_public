using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class DeviceTypePartConfiguration : IEntityTypeConfiguration<DeviceTypePart>
    {
        public void Configure(EntityTypeBuilder<DeviceTypePart> modelBuilder)
        {
            modelBuilder.HasKey(tp => new { tp.DeviceTypeId, tp.DevicePartId });


            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}
