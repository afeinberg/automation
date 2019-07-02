using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace automation.Models
{
    public class TelemetryDbContext : DbContext
    {

        public TelemetryDbContext(DbContextOptions<TelemetryDbContext> opts)
            : base(opts)
        {
        }

        public DbSet<TelemetryData> TelemetryData { get; set; }

        public DbSet<Sensor> Sensor { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TelemetryData>()
                .Property(p => p.Id)
                .ForNpgsqlUseSequenceHiLo();
            modelBuilder.Entity<Sensor>()
                .Property(p => p.Id)
                .ForNpgsqlUseSequenceHiLo();
        }
    }
}
