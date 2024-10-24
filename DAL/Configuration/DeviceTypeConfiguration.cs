using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class DeviceTypeConfiguration : IEntityTypeConfiguration<DeviceType>
    {
        public void Configure(EntityTypeBuilder<DeviceType> modelBuilder)
        {
            modelBuilder.HasIndex(dt => dt.Name)
                .IsUnique();

            modelBuilder.HasMany(dt => dt.Alarms)
                .WithMany(a => a.DeviceTypes)
                .UsingEntity<DeviceTypeAlarm>();

            modelBuilder.HasMany(dt => dt.DeviceParts)
                .WithMany(dp => dp.DeviceTypes)
                .UsingEntity<DeviceTypePart>();


            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}
