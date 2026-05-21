using Microsoft.Win32;
using System;
using System.IO; // FileStream için gerekli
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu.Login
{
    public partial class Login : Window
    {
        private readonly AppDbContext _context = new AppDbContext();

        public Login()
        {
            InitializeComponent();
            SistemAdiniGetir(); // Sayfa açılırken veritabanından başlığı çeker
            LogoYukle(); // Sayfa açılırken veritabanından logoyu çeker
        }

        // --- SİSTEM ADINI GETİREN METOT ---
        private void SistemAdiniGetir()
        {
            try
            {
                var ayarlar = _context.SistemAyarlari.FirstOrDefault();
                if (ayarlar != null && !string.IsNullOrEmpty(ayarlar.SistemAdi))
                {
                    this.Title = ayarlar.SistemAdi; // Pencere sekme başlığı

                    if (txtLoginBaslik != null)
                    {
                        txtLoginBaslik.Text = ayarlar.SistemAdi; // Ekrandaki büyük yazı
                    }
                }
            }
            catch
            {
                // Veritabanı henüz yoksa veya bağlantı koptuysa hata vermesin, XAML'daki varsayılan isim kalsın
            }
        }

        // --- LOGOYU GETİREN YENİ METOT ---
        private void LogoYukle()
        {
            try
            {
                var ayarlar = _context.SistemAyarlari.FirstOrDefault();
                if (ayarlar != null && !string.IsNullOrWhiteSpace(ayarlar.LogoYolu) && System.IO.File.Exists(ayarlar.LogoYolu))
                {
                    var bitmap = new BitmapImage();

                    // DÜZELTME: Dosya kilitlenmesini kesin olarak önlemek için FileStream akışı ile güvenle yükleniyor
                    using (FileStream fs = new FileStream(ayarlar.LogoYolu, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        bitmap.BeginInit();
                        bitmap.StreamSource = fs;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad; // Resmi tamamen RAM'e yükler
                        bitmap.EndInit();
                    }
                    bitmap.Freeze(); // İş parçacıkları arası güvenli paylaşım sağlar

                    // Ekrandaki Login Image nesnesine logoyu bas
                    if (imgLoginLogo != null) imgLoginLogo.Source = bitmap;

                    // Pencerenin sol üst ikonunu logoyla değiştir
                    this.Icon = bitmap;
                }
            }
            catch
            {
                // Logo yüklenirken bir hata olursa (dosya bozuk vs.) program çökmesin
            }
        }

        private void btnGiris_Click(object sender, RoutedEventArgs e)
        {
            string kadi = txtKullanici.Text;
            string sifre = txtSifre.Password;

            if (string.IsNullOrWhiteSpace(kadi) || string.IsNullOrWhiteSpace(sifre))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 1. Önce veritabanından kullanıcı adına benzeyen aktif kayıtları çek (SQL büyük/küçük harf ayırmaz)
                var dbKullanicilar = _context.Kullanicilar
                    .Where(u => u.KullaniciAdi == kadi && u.AktifMi == true)
                    .ToList();

                // 2. C# tarafında (RAM'de) Büyük/Küçük harf duyarlı (Case-Sensitive) kesin eşleşmeyi yap
                var kullanici = dbKullanicilar
                    .FirstOrDefault(u => u.KullaniciAdi == kadi && u.Sifre == sifre);

                if (kullanici != null)
                {
                    // --- OTURUMU BAŞLAT ---
                    OturumYonetimi.GirisYapanKullanici = kullanici;

                    // Giriş başarılı, Main penceresini aç
                    Main anaPencere = new Main();
                    anaPencere.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya şifre hatalı ya da hesabınız pasif!", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantı hatası: " + ex.Message);
            }
        }

        private void btnKapat_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}