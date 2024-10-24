using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class SectionDirectionConfiguration : IEntityTypeConfiguration<SectionDirection>
    {
        public void Configure(EntityTypeBuilder<SectionDirection> modelBuilder)
        {
            modelBuilder.HasIndex(d => d.Name)
                .IsUnique();

            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}
