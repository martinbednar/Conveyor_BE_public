using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class MonitoredItemConfiguration : IEntityTypeConfiguration<MonitoredItem>
    {
        public void Configure(EntityTypeBuilder<MonitoredItem> modelBuilder)
        {
            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}
