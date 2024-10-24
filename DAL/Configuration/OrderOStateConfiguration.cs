using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class OrderOStateConfiguration : IEntityTypeConfiguration<OrderOState>
    {
        public void Configure(EntityTypeBuilder<OrderOState> modelBuilder)
        {
            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}
