using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class DeviceMonitoredItemConfiguration : IEntityTypeConfiguration<DeviceMonitoredItem>
    {
        public void Configure(EntityTypeBuilder<DeviceMonitoredItem> modelBuilder)
        {
            modelBuilder.HasKey(di => new { di.DeviceId, di.MonitoredItemId });

            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}
