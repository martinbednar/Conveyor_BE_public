using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Configuration
{
    // Used for seed initial data.
    // Tutorial: https://code-maze.com/migrations-and-seed-data-efcore/
    internal class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> modelBuilder)
        {
            modelBuilder.HasIndex(s => s.Name)
                .IsUnique();
            modelBuilder.HasIndex(s => new { s.InputDeviceId, s.OutputDeviceId })
                .IsUnique();
            modelBuilder.HasIndex(s => new { s.InputDeviceId, s.SectionDirectionFromInputDeviceId })
                .IsUnique();
            modelBuilder.HasIndex(s => new { s.OutputDeviceId, s.SectionDirectionToOutputDeviceId })
                .IsUnique();
            modelBuilder.HasOne(s => s.OutputDevice)
                .WithMany(d => d.PreviousSections)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.HasOne(s => s.InputDevice)
                .WithMany(d => d.NextSections)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.HasOne(s => s.SectionDirectionToOutputDevice)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.HasOne(s => s.SectionDirectionFromInputDevice)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.HasData(
                // PRIVATE - NOT PUBLISHED
            );
        }
    }
}
