using Microsoft.EntityFrameworkCore; 
using EnerjiTahmin.Models; 

namespace EnerjiTahmin.Data 
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tablolar
        public DbSet<Suggestion> Oneriler { get; set; }
        public DbSet<TuketimVerisi> TuketimVerileri { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<IslemLog> IslemLoglari { get; set; }

        // View'ler (Buraya eklediklerin)
        public DbSet<UserDetailView> UserDetails { get; set; }
        public DbSet<CityTempView> CityTemperatures { get; set; }
        public DbSet<DailySummaryView> DailySummaries { get; set; }
        public DbSet<HighConsumptionView> HighConsumptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. UserDetails View Ayarı
            modelBuilder.Entity<UserDetailView>(entity =>
            {
                entity.HasNoKey(); 
                entity.ToView("vw_userdetails"); 
            });
            
            // 2. CityTemperatures View Ayarı
            modelBuilder.Entity<CityTempView>(entity =>
            {
                entity.HasNoKey(); 
                entity.ToView("vw_cityaveragetemperature"); 
            });
            modelBuilder.Entity<DailySummaryView>(entity =>
    {
        entity.HasNoKey();
        entity.ToView("vw_dailyconsumptionsummary");
    });
    modelBuilder.Entity<HighConsumptionView>(entity =>
    {
        entity.HasNoKey();
        entity.ToView("vw_highconsumptionhours");
    });

            // TEK VE SON ÇAĞRI (Doğrusu budur)
            base.OnModelCreating(modelBuilder);
        }
    }
}