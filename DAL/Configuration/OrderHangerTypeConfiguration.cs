using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class OrderHangerTypeConfiguration : IEntityTypeConfiguration<OrderHangerType>
    {
        public void Configure(EntityTypeBuilder<OrderHangerType> modelBuilder)
        {
            modelBuilder.HasIndex(t => t.Name)
                .IsUnique();

            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}
