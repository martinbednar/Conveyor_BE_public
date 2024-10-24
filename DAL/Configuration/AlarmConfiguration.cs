using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class AlarmConfiguration : IEntityTypeConfiguration<Alarm>
    {
        public void Configure(EntityTypeBuilder<Alarm> modelBuilder)
        {
            modelBuilder.HasIndex(a => a.Name)
                .IsUnique();


            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}
