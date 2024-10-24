using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class StationTypeConfiguration : IEntityTypeConfiguration<StationType>
    {
        public void Configure(EntityTypeBuilder<StationType> modelBuilder)
        {
            modelBuilder.HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}
