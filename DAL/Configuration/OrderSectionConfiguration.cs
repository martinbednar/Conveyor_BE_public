using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class OrderSectionConfiguration : IEntityTypeConfiguration<OrderSection>
    {
        public void Configure(EntityTypeBuilder<OrderSection> modelBuilder)
        {
            modelBuilder.HasKey(os => new { os.OrderId, os.StationId });
        }
    }
}
