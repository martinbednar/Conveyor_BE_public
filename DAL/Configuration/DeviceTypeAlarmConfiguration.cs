using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class DeviceTypeAlarmConfiguration : IEntityTypeConfiguration<DeviceTypeAlarm>
    {
        public void Configure(EntityTypeBuilder<DeviceTypeAlarm> modelBuilder)
        {
            modelBuilder.HasKey(da => new { da.DeviceTypeId, da.AlarmId });


            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}
