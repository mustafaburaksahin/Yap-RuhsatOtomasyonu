using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YapıRuhsatOtomasyonu.Data; // Veritabanı bağlantısı için eklendi

namespace YapıRuhsatOtomasyonu.YapiBilgileri
{
    /// <summary>
    /// PDF.xaml etkileşim mantığı
    /// </summary>
    public partial class PDF : Page // Dikkat: Window yerine Page yaptık!
    {
        // Yakınlaştırma seviyesini tutacağımız değişken (Başlangıç 1.0 = %100)
        private double zoomLevel = 1.0;

        public PDF()
        {
            InitializeComponent();
            KurumBilgileriniYukle(); // Sayfa açılırken Sistem Ayarlarını çek
        }

        // --- SİSTEM AYARLARINDAN BELEDİYE, İL VE İLÇE ADINI ÇEKME METODU ---
        private void KurumBilgileriniYukle()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var ayar = context.SistemAyarlari.FirstOrDefault();

                    if (ayar != null)
                    {
                        // 1. Belediye Adı Kontrolü
                        if (!string.IsNullOrWhiteSpace(ayar.BelediyeAdi) && txtRuhsatVerenKurum != null)
                        {
                            txtRuhsatVerenKurum.Text = ayar.BelediyeAdi.Trim();
                        }

                        // 2. İl Kontrolü
                        if (!string.IsNullOrWhiteSpace(ayar.Il) && txtIl != null)
                        {
                            txtIl.Text = ayar.Il.Trim();
                        }

                        // 3. İlçe Kontrolü
                        if (!string.IsNullOrWhiteSpace(ayar.Ilce) && txtIlce != null)
                        {
                            txtIlce.Text = ayar.Ilce.Trim();
                        }

                        // 4. Logo Kontrolü ve Ekrana Basma
                        if (!string.IsNullOrWhiteSpace(ayar.LogoYolu) && System.IO.File.Exists(ayar.LogoYolu) && imgKurumLogo != null)
                        {
                            try
                            {
                                var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                                bitmap.BeginInit();
                                bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad; // Resmi kitlememesi için
                                bitmap.UriSource = new Uri(ayar.LogoYolu);
                                bitmap.EndInit();

                                imgKurumLogo.Source = bitmap;
                            }
                            catch
                            {
                                // Logo yüklenirken bir hata olursa (dosya bozuk vs.) program çökmesin
                            }
                        }
                    }
                }
            }
            catch
            {
                // Veritabanına bağlanılamazsa veya hata çıkarsa program çökmesin, noktalar kalsın.
            }
        }

        // --- ZOOM (YAKINLAŞTIRMA/UZAKLAŞTIRMA) METOTLARI ---
        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (zoomLevel < 3.0) // Maksimum %300 yakınlaştırma limiti
            {
                zoomLevel += 0.2; // Her tıklamada %20 büyüt
                ZoomUygula();
            }
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (zoomLevel > 0.4) // Minimum %40 uzaklaştırma limiti
            {
                zoomLevel -= 0.2; // Her tıklamada %20 küçült
                ZoomUygula();
            }
        }

        private void BtnZoomReset_Click(object sender, RoutedEventArgs e)
        {
            zoomLevel = 1.0; // %100'e (Orijinal boyuta) geri dön
            ZoomUygula();
        }

        private void ZoomUygula()
        {
            // 1. Yakınlaştırma motorunu güncelle (Eğer burada hata veriyorsa XAML'da adı eksiktir)
            if (pdfScaleTransform != null)
            {
                pdfScaleTransform.ScaleX = zoomLevel;
                pdfScaleTransform.ScaleY = zoomLevel;
            }

            // 2. Görseldeki isimle eşleşen TextBlock'a yüzdeyi yazdır
            if (txtZoomLevel != null)
            {
                txtZoomLevel.Text = $"%{(int)(Math.Round(zoomLevel, 2) * 100)}";
            }
        }

        // --- VERİ YÜKLEME VE DOLDURMA İŞLEMLERİ ---
        // İleride DataGrid'den seçtiğin verileri bu PDF formuna doldurmak için bu metodu kullanacağız
        public void VerileriYukle(dynamic yapi, dynamic detay)
        {
            if (yapi != null)
            {
                try
                {
                    // 1. Alt Tablodaki Pafta, Ada, Parsel, YKN Bilgileri
                    if (vPafta != null) vPafta.Text = yapi.PaftaNo != null ? yapi.PaftaNo.ToString() : "-";
                    if (vAda != null) vAda.Text = yapi.Ada != null ? yapi.Ada.ToString() : "-";
                    if (vParsel != null) vParsel.Text = yapi.Parsel != null ? yapi.Parsel.ToString() : "-";
                    if (vYKN != null) vYKN.Text = yapi.YapiKimlikNo != null ? yapi.YapiKimlikNo.ToString() : "-";

                    // İstersen blok kısmını da buradan bağlayabilirsin:
                    // if (vBlok != null) vBlok.Text = yapi.Blok != null ? yapi.Blok.ToString() : "-";

                    // 2. Mahalle/Mevki TextBox'ı
                    if (txtMahalle != null)
                    {
                        txtMahalle.Text = yapi.Mahalle != null ? yapi.Mahalle.ToString() : "";
                    }

                    // 3. Cadde/Sokak/Bulvar TextBox'ı (Birleştirilmiş Format)
                    if (txtCaddeSokak != null)
                    {
                        // Mahalle / Cadde / Sokak şeklinde metni birleştiriyoruz
                        string adresBirlesim = $"{yapi.Mahalle} / {yapi.Cadde} / {yapi.Sokak}";

                        // Eğer hepsi boşsa sadece " /  / " yazdırmasın diye ufak bir kontrol:
                        if (string.IsNullOrWhiteSpace(adresBirlesim.Replace("/", "").Trim()))
                        {
                            txtCaddeSokak.Text = "";
                        }
                        else
                        {
                            txtCaddeSokak.Text = adresBirlesim;
                        }
                    }
                }
                catch
                {
                    // Veri çekilirken özellik eşleşmezse çökmesini engeller
                }
            }
        }

        // --- KULLANICI DENEYİMİ (UX) İÇİN TEXTBOX SEÇİM EVENTLERİ ---
        // Kullanıcı düzenlemek için bu alanlara tıkladığında metnin tamamı seçilir.

        private void txtCaddeSokak_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null && !tb.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tb.Focus();
                tb.SelectAll(); // Tıklandığında tüm metni otomatik seç
            }
        }

        // Eğer Mahalle kısmına da bu özelliği XAML'da (PreviewMouseDown="txtMahalle_PreviewMouseDown") eklersen diye metodu bırakıyorum.
        private void txtMahalle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null && !tb.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tb.Focus();
                tb.SelectAll(); // Tıklandığında tüm metni otomatik seç
            }
        }
    }
}