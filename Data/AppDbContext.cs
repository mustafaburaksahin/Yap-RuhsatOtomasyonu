using Microsoft.EntityFrameworkCore;
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<BelgeTipiModel> BelgeTipleri { get; set; }
        public DbSet<DosyaTipiModel> DosyaTipleri { get; set; }
        public DbSet<RuhsatAmaciModel> RuhsatAmaclari { get; set; }
        public DbSet<IskanAmaciModel> IskanAmaclari { get; set; }
        public DbSet<SicilModel> Siciller { get; set; }
        public DbSet<SicilTipiModel> SicilTipleri { get; set; }
        public DbSet<KullaniciModel> Kullanicilar { get; set; }
        public DbSet<UnvanModel> Unvanlar { get; set; }
        public DbSet<YapiModel> Yapilar { get; set; }
        public DbSet<BirimModel> Birimler { get; set; }
        public DbSet<DosyaModel> Dosyalar { get; set; }
        public DbSet<YetkiModel> Yetkiler { get; set; }
        public DbSet<BelgeDetayModel> BelgeDetay { get; set; }
        public DbSet<DosyaDetayModel> DosyaDetaylar { get; set; }
        public DbSet<SistemAyarModel> SistemAyarlari { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // SQL Server bağlantı dizesi
            //optionsBuilder.UseSqlServer(@"Server=DESKTOP-BELEDIYE\SQLEXPRESS;Database=YapiRuhsatDb;Trusted_Connection=True;TrustServerCertificate=True;");
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=YapiRuhsatDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Veritabanı tablo isimleri model isimlerinden farklıysa burada eşleme yapabilirsin.
            // Örnek: modelBuilder.Entity<DosyaDetayModel>().ToTable("DosyaDetay");
        }
    }
}