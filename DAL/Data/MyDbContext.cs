using DAL.Configuration;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class MyDbContext : DbContext
    {
        public DbSet<OrderHangerType> OrderHangerTypes { get; set; }
        public DbSet<OState> OStates { get; set; }
        public DbSet<OrderOState> OrderStates { get; set; }
        public DbSet<StationType> StationTypes { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<MonitoredItem> MonitoredItems { get; set; }
        public DbSet<DeviceMonitoredItem> DeviceMonitoredItems { get; set; }
        public DbSet<SectionDirection> SectionDirections { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<OrderHanger> OrderHangers { get; set; }
        public DbSet<OrderHangerSection> OrderHangerSections { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderSection> OrderSections { get; set; }
        public DbSet<OpcUaLog> OpcUaLogs { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<DeviceTypeAlarm> DeviceTypeAlarms { get; set; }
        public DbSet<DeviceAlarmLog> DeviceAlarmLogs { get; set; }
        public DbSet<DevicePart> DeviceParts { get; set; }
        public DbSet<DeviceTypePart> DeviceTypeParts { get; set; }
        public DbSet<RfidHeadLog> RfidHeadLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Place the connection string here.");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DeviceConfiguration());
            modelBuilder.ApplyConfiguration(new MonitoredItemConfiguration());
            modelBuilder.ApplyConfiguration(new DeviceMonitoredItemConfiguration());
            modelBuilder.ApplyConfiguration(new OrderSectionConfiguration());
            modelBuilder.ApplyConfiguration(new StationTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OStateConfiguration());
            modelBuilder.ApplyConfiguration(new SectionDirectionConfiguration());
            modelBuilder.ApplyConfiguration(new SectionConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderHangerTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderOStateConfiguration());
            modelBuilder.ApplyConfiguration(new DeviceTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AlarmConfiguration());
            modelBuilder.ApplyConfiguration(new DeviceTypeAlarmConfiguration());
            modelBuilder.ApplyConfiguration(new DevicePartConfiguration());
            modelBuilder.ApplyConfiguration(new DeviceTypePartConfiguration());
        }
    }
}
