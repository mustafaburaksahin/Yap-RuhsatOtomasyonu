using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Models;
using YapiRuhsatOtomasyonu.Pages;
using YapıRuhsatOtomasyonu.Login;

namespace YapıRuhsatOtomasyonu.YapiBilgileri
{
    public partial class DosyaListesi : Window
    {
        private readonly AppDbContext _context = new AppDbContext();
        private int _gelenYapiId;
        private int _gorunenYapiSiraNo;

        private readonly string _guvenliAnaKlasor = @"C:\YapiRuhsatArsiv\Arsiv";

        public static readonly DependencyProperty AdminVisibilityProperty =
            DependencyProperty.Register("AdminVisibility", typeof(Visibility), typeof(DosyaListesi), new PropertyMetadata(Visibility.Collapsed));

        public Visibility AdminVisibility
        {
            get { return (Visibility)GetValue(AdminVisibilityProperty); }
            set { SetValue(AdminVisibilityProperty, value); }
        }

        public DosyaListesi(int yapiId, int gorunenSiraNo = 0)
        {
            InitializeComponent();
            _gelenYapiId = yapiId;
            _gorunenYapiSiraNo = gorunenSiraNo;

            // YAPILAN GÜNCELLEME: Arayüzdeki (ID): yazan kısma gelen YapiId'yi atıyoruz.
            txtYapiId.Text = $" (ID: {_gelenYapiId})";

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
                var liste = _context.DosyaDetaylar
                    .Include(x => x.DosyaTipi)
                    .Where(x => x.YapiSiraNo == _gelenYapiId)
                    .OrderByDescending(x => x.KayitTarihi)
                    .ToList();

                dgDosyalar.ItemsSource = liste;

                if (liste.Count == 0)
                {
                    MessageBox.Show("Bu yapıya ait herhangi bir teknik dosya bulunamadı.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void BtnDosyaAc_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is DosyaDetayModel secilen)
            {
                string kesinYol = secilen.DosyaYolu ?? "";

                if (kesinYol.Contains("Yeni Klasör") || kesinYol.Contains("Masaüstü"))
                {
                    kesinYol = Path.Combine(_guvenliAnaKlasor, _gelenYapiId.ToString(), Path.GetFileName(kesinYol));
                }

                if (!string.IsNullOrEmpty(kesinYol) && File.Exists(kesinYol))
                {
                    try
                    {
                        string uzanti = Path.GetExtension(kesinYol).ToLower();

                        if (uzanti == ".zip" || uzanti == ".rar")
                        {
                            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
                            {
                                Title = "Dosyayı İndir",
                                FileName = Path.GetFileName(kesinYol),
                                Filter = "Sıkıştırılmış Dosya|*" + uzanti + "|Tüm Dosyalar (*.*)|*.*"
                            };

                            if (saveFileDialog.ShowDialog() == true)
                            {
                                File.Copy(kesinYol, saveFileDialog.FileName, true);
                                MessageBox.Show("Dosya başarıyla indirildi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(kesinYol) { UseShellExecute = true });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Dosya işlemi sırasında hata oluştu: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show($"Dosya bulunamadı veya ağ bağlantısı koptu.\n\nAranan Yol:\n{kesinYol}", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnDosyaSil_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is DosyaDetayModel secilen)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"'{secilen.Aciklama}' başlıklı dosyayı arşive taşıyıp listeden kaldırmak üzeresiniz. Onaylıyor musunuz?",
                    "Dosya Arşivleme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var silinecek = _context.DosyaDetaylar.Find(secilen.SiraNo);
                        if (silinecek != null)
                        {
                            string kaynakYol = secilen.DosyaYolu ?? "";

                            if (kaynakYol.Contains("Yeni Klasör") || kaynakYol.Contains("Masaüstü"))
                            {
                                kaynakYol = Path.Combine(_guvenliAnaKlasor, _gelenYapiId.ToString(), Path.GetFileName(kaynakYol));
                            }

                            if (!string.IsNullOrEmpty(kaynakYol) && File.Exists(kaynakYol))
                            {
                                try
                                {
                                    string yapiSahibiTam = _context.Yapilar.FirstOrDefault(y => y.Id == _gelenYapiId)?.YapiSahibi ?? "Bilinmiyor";
                                    string yapiSahibi = yapiSahibiTam.Split('-')[0].Trim().Replace(" ", "");
                                    string dosyaTipiAd = secilen.DosyaTipi?.Ad?.Replace(" ", "") ?? "BelirsizTip";
                                    string orijinalAd = Path.GetFileNameWithoutExtension(kaynakYol);
                                    string uzanti = Path.GetExtension(kaynakYol);
                                    string tarih = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                                    string yeniDosyaAdi = $"{_gelenYapiId}_{yapiSahibi}_{dosyaTipiAd}_{orijinalAd}_{tarih}{uzanti}";

                                    if (!Directory.Exists(_guvenliAnaKlasor)) Directory.CreateDirectory(_guvenliAnaKlasor);

                                    string hedefTamYol = Path.Combine(_guvenliAnaKlasor, yeniDosyaAdi);

                                    File.Move(kaynakYol, hedefTamYol);
                                }
                                catch (Exception fileEx)
                                {
                                    MessageBox.Show($"Dosya arşive taşınırken hata oluştu (Dosya kullanımda olabilir):\n{fileEx.Message}", "İşlem Durduruldu", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                            }

                            _context.DosyaDetaylar.Remove(silinecek);
                            _context.SaveChanges();

                            MessageBox.Show("Dosya başarıyla arşive taşındı.");
                            VerileriYukle();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Silme işlemi sırasında bir hata oluştu: " + ex.Message);
                    }
                }
            }
        }
    }
}