using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Entities;
using WebApplication1.Services;

namespace WebApplication1.Data
{

    public class DataContext : DbContext
    {
        public DbSet<AskEntity> Asks { get; set; }
        public DbSet<LastEntity> Lasts { get; set; }
        public DbSet<BidEntity> Bids { get; set; }
        public DbSet<InstrumentsEntity> Instruments { get; set; }
        public DbSet<InsturmentMappingEntity> InstrumentMappings { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=app.db");

        }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InsturmentMappingEntity>()
                .HasKey(im => new { im.InstrumentId, im.MappingType });

            modelBuilder.Entity<InsturmentMappingEntity>()
                .HasOne(im => im.Instrument)
                .WithMany()
                .HasForeignKey(im => im.InstrumentId);

            modelBuilder.Entity<BidEntity>()
            .HasKey(b => new { b.InstrumentId, b.Timestamp });

            modelBuilder.Entity<BidEntity>()
                .HasOne(b => b.Instrument)
                .WithMany()
                .HasForeignKey(b => b.InstrumentId);

            modelBuilder.Entity<AskEntity>()
            .HasKey(b => new { b.InstrumentId, b.Timestamp });

            modelBuilder.Entity<AskEntity>()
                .HasOne(b => b.Instrument)
                .WithMany()
                .HasForeignKey(b => b.InstrumentId);

            modelBuilder.Entity<LastEntity>()
            .HasKey(b => new { b.InstrumentId, b.Timestamp });

            modelBuilder.Entity<LastEntity>()
                .HasOne(b => b.Instrument)
                .WithMany()
                .HasForeignKey(b => b.InstrumentId);
        }

        public static void InitializeDatabase()
        {
            // Create a new instance of DataContext
            using (var context = new DataContext())
            {
                // Ensure the database is created
                context.Database.EnsureCreated();
                // Alternatively, if using migrations:
                // context.Database.Migrate();
            }
        }

        public static async Task GetInstrumentsAndAddToTable(){
            await FintaChartsService.GetInsturments("oanda", "forex");
        }

    }
}
