using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> modelBuilder)
        {
            modelBuilder.HasIndex(d => d.Name)
                .IsUnique();
            modelBuilder.HasIndex(d => d.NodeId)
                .IsUnique();
            modelBuilder.HasMany(d => d.PreviousSections)
                .WithOne(s => s.OutputDevice);
            modelBuilder.HasMany(d => d.NextSections)
                .WithOne(s => s.InputDevice);

            modelBuilder.HasMany(d => d.MonitoredItems)
                .WithMany(i => i.Devices)
                .UsingEntity<DeviceMonitoredItem>();


            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}
