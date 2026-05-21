using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Uygulama başlarken veritabanı ve yönetici kontrolünü yap
            KurucuHesabiOlustur();
        }

        private void KurucuHesabiOlustur()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // 1. ADIM: VERİTABANI OLUŞTURMA VEYA GÜNCELLEME
                    // Eğer Migration (Update-Database) kullanıyorsan Migrate() kullan:
                    // db.Database.Migrate(); 

                    // Eğer Migration kullanmıyorsan senin yazdığın bu kod en garantisidir:
                    db.Database.EnsureCreated();

                    // 2. ADIM: KURUCU HESAP KONTROLÜ
                    var kurucuVarMi = db.Kullanicilar.Any(u => u.KullaniciAdi == "admin");

                    if (!kurucuVarMi)
                    {
                        var kurucu = new KullaniciModel
                        {
                            AdSoyad = "Mustafa Burak Şahin", // Kurucu yetkilisi sensin!
                            KullaniciAdi = "admin",
                            Sifre = "123",
                            KullaniciTipi = "Admin",
                            AktifMi = true,
                            KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                            KaydedenKullanici = "Sistem"
                        };

                        db.Kullanicilar.Add(kurucu);
                        db.SaveChanges();

                        System.Diagnostics.Debug.WriteLine("Kurucu hesap (admin) başarıyla oluşturuldu.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Ağdaki SQL'e ulaşılamazsa veya kapalıysa bu hata patlar, sebebi anlamanı sağlar.
                MessageBox.Show("Veritabanına bağlanılamadı!\n\nAna makinenin açık olduğundan ve ağ ayarlarının (IP, Firewall, SQL TCP/IP) yapıldığından emin olun.\n\nHata Detayı: " + ex.Message,
                                "Kritik Veritabanı Hatası",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                // Veritabanı yoksa programın yarım yamalak açılmasını engeller
                Current.Shutdown();
            }
        }
    }
}