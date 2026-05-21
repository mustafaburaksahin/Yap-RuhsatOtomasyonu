using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Models;
using Microsoft.EntityFrameworkCore;
using YapıRuhsatOtomasyonu.Login;

namespace YapıRuhsatOtomasyonu.Pages
{
    public partial class BelgeListesi : Window
    {
        private readonly AppDbContext _context = new AppDbContext();
        private int _gelenYapiId;

        private readonly string _guvenliBelgeKlasor = @"C:\YapiRuhsatArsiv\Belgeler";
        private readonly string _guvenliAnaKlasor = @"C:\YapiRuhsatArsiv\Arsiv";

        public static readonly DependencyProperty AdminVisibilityProperty =
            DependencyProperty.Register("AdminVisibility", typeof(Visibility), typeof(BelgeListesi), new PropertyMetadata(Visibility.Collapsed));

        public Visibility AdminVisibility
        {
            get { return (Visibility)GetValue(AdminVisibilityProperty); }
            set { SetValue(AdminVisibilityProperty, value); }
        }

        public BelgeListesi(int yapiId)
        {
            InitializeComponent();
            _gelenYapiId = yapiId;
            txtYapiId.Text = $" - Yapının ID : {_gelenYapiId}";

            // --- YETKİ KONTROLÜ ---
            // AdminPanel > Yapılar > Sil butonu aktifse (SilYetki == true) göster, değilse gizle.
            AdminVisibility = Visibility.Collapsed;

            var user = OturumYonetimi.GirisYapanKullanici;
            if (user != null)
            {
                if (user.KullaniciAdi == "admin" || user.KullaniciTipi == "Yönetici")
                {
                    AdminVisibility = Visibility.Visible;
                }
                else
                {
                    var yapiYetkisi = _context.Yetkiler.FirstOrDefault(x =>
                        x.KullaniciAdi == user.KullaniciAdi &&
                        x.Panel == "Yapılar" &&
                        x.SilYetki == true);

                    if (yapiYetkisi != null)
                    {
                        AdminVisibility = Visibility.Visible;
                    }
                }
            }

            this.DataContext = this;
            VerileriYukle();
        }

        private void VerileriYukle()
        {
            try
            {
                var liste = _context.BelgeDetay
                    .Include(x => x.RuhsatAmaci)
                    .Include(x => x.IskanAmaci)
                    .Where(x => x.YapiSiraNo == _gelenYapiId)
                    .OrderByDescending(x => x.KayitTarihi)
                    .ToList();

                dgBelgeListesi.ItemsSource = liste;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Liste yüklenirken hata: " + ex.Message);
            }
        }

        private void BtnDosyaAc_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is BelgeDetayModel secilen)
            {
                if (!string.IsNullOrEmpty(secilen.BelgeYolu))
                {
                    string kesinYol = secilen.BelgeYolu;

                    if (kesinYol.Contains("Yeni Klasör") || kesinYol.Contains("Masaüstü"))
                    {
                        kesinYol = Path.Combine(_guvenliBelgeKlasor, _gelenYapiId.ToString(), secilen.BelgeAdi ?? "");
                    }

                    if (File.Exists(kesinYol))
                    {
                        Process.Start(new ProcessStartInfo(kesinYol) { UseShellExecute = true });
                    }
                    else if (File.Exists(secilen.BelgeYolu))
                    {
                        Process.Start(new ProcessStartInfo(secilen.BelgeYolu) { UseShellExecute = true });
                    }
                    else
                    {
                        MessageBox.Show($"Dosya bulunamadı!\n\nAranan Konum:\n{kesinYol}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Seçilen belgeye ait bir yol bilgisi veritabanında yok!", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnBelgeSil_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is BelgeDetayModel secilen)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"DİKKAT: '{secilen.BelgeAdi}' isimli belge arşive taşınacak ve listeden kaldırılacak.\nEmin misiniz?",
                    "Kritik Onay", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var silinecek = _context.BelgeDetay.Find(secilen.SiraNo);
                        if (silinecek != null)
                        {
                            if (!string.IsNullOrEmpty(secilen.BelgeYolu))
                            {
                                string kaynakYol = secilen.BelgeYolu;

                                if (kaynakYol.Contains("Yeni Klasör") || kaynakYol.Contains("Masaüstü"))
                                {
                                    kaynakYol = Path.Combine(_guvenliBelgeKlasor, _gelenYapiId.ToString(), secilen.BelgeAdi ?? "");
                                }

                                if (!File.Exists(kaynakYol) && File.Exists(secilen.BelgeYolu))
                                {
                                    kaynakYol = secilen.BelgeYolu;
                                }

                                if (File.Exists(kaynakYol))
                                {
                                    try
                                    {
                                        string yapiSahibiTam = _context.Yapilar.FirstOrDefault(y => y.Id == _gelenYapiId)?.YapiSahibi ?? "Bilinmiyor";
                                        string yapiSahibi = yapiSahibiTam.Split('-')[0].Trim().Replace(" ", "");
                                        string belgeTuru = secilen.RuhsatVerilisAmaciId?.ToString().Replace(" ", "") ?? "BilinmeyenBelge";
                                        string orijinalAd = Path.GetFileNameWithoutExtension(kaynakYol);
                                        string uzanti = Path.GetExtension(kaynakYol);
                                        string tarih = DateTime.Now.ToString("yyyyMMdd_HHmm");

                                        string yeniDosyaAdi = $"{_gelenYapiId}_{yapiSahibi}_{belgeTuru}_{orijinalAd}_{tarih}{uzanti}";

                                        if (!Directory.Exists(_guvenliAnaKlasor)) Directory.CreateDirectory(_guvenliAnaKlasor);

                                        string hedefTamYol = Path.Combine(_guvenliAnaKlasor, yeniDosyaAdi);

                                        File.Move(kaynakYol, hedefTamYol);
                                    }
                                    catch (Exception fileEx)
                                    {
                                        MessageBox.Show($"Dosya arşive taşınırken hata oluştu (Belge açık olabilir): {fileEx.Message}", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        return;
                                    }
                                }
                            }

                            _context.BelgeDetay.Remove(silinecek);
                            _context.SaveChanges();

                            MessageBox.Show("Belge başarıyla listelerden kaldırıldı ve Arşiv klasörüne taşındı.");
                            VerileriYukle();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("İşlem başarısız: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}