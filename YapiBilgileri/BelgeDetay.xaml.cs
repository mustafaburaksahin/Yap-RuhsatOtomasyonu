using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Models;
using YapıRuhsatOtomasyonu.Pages;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using YapıRuhsatOtomasyonu.Login;

namespace YapıRuhsatOtomasyonu.Pages
{
    public partial class BelgeDetay : Window
    {
        private readonly AppDbContext _context = new AppDbContext();
        private YapiModel? _secilenYapi;
        private int _siraNo;

        // GÜNCELLEME: Düzenleme modu için mevcut belge tutucusu eklendi
        private BelgeDetayModel? _mevcutBelge;

        private readonly string _guvenliBelgeKlasor = @"C:\YapiRuhsatArsiv\Belgeler";

        // GÜNCELLEME: Constructor'a mevcutBelge parametresi eklendi (Geriye dönük uyumluluğu bozmaz)
        public BelgeDetay(YapiModel? yapi, int siraNo, string dosyaYolu = "", BelgeDetayModel? mevcutBelge = null)
        {
            InitializeComponent();

            _secilenYapi = yapi ?? new YapiModel();
            _siraNo = siraNo;
            _mevcutBelge = mevcutBelge;

            txtId.Text = _siraNo.ToString();
            txtYapiSahibi.Text = _secilenYapi?.YapiSahibi ?? "";

            VerileriYukle();

            // GÜNCELLEME: Eğer bu sayfa bir "Düzenleme" işlemi için açıldıysa verileri doldur
            if (_mevcutBelge != null)
            {
                cmbRuhsatVerilis.SelectedValue = _mevcutBelge.RuhsatVerilisAmaciId;
                cmbIskanAmaci.SelectedValue = _mevcutBelge.IskanVerilisAmaciId;
                txtRuhsatNo.Text = _mevcutBelge.RuhsatNo;
                txtYapiKimlikNo.Text = _mevcutBelge.YapiKimlikNo;
                txtRuhsatBilgileri.Text = _mevcutBelge.RuhsatBilgileri;
                txtRuhsatTarihi.Text = _mevcutBelge.RuhsatTarihi;
                txtIskanTarihi.Text = _mevcutBelge.IskanTarihi;
                txtEsasRuhsat.Text = _mevcutBelge.BelgeyeEsasRuhsat;
                txtAciklama.Text = _mevcutBelge.Aciklama;
                txtBelgeAdi.Text = _mevcutBelge.BelgeAdi;
                txtBelgeYolu.Text = _mevcutBelge.BelgeYolu;

                string kontrolYolu = _mevcutBelge.BelgeYolu ?? "";
                if (kontrolYolu.Contains("Yeni Klasör") || kontrolYolu.Contains("Masaüstü"))
                {
                    kontrolYolu = Path.Combine(_guvenliBelgeKlasor, _secilenYapi!.Id.ToString(), _mevcutBelge.BelgeAdi ?? "");
                }
                OnizlemeYukle(kontrolYolu, GetWbOnizleme());
            }
            else
            {
                // Yeni Ekleme Modu
                txtBelgeYolu.Text = dosyaYolu;
                txtBelgeAdi.Text = Path.GetFileName(dosyaYolu);

                if (!string.IsNullOrEmpty(dosyaYolu))
                {
                    OnizlemeYukle(dosyaYolu, GetWbOnizleme());
                }
            }

            // DÜZELTME: Eski kaydı otomatik bulup getiren ve birden fazla kayıt eklenmesini engelleyen "SelectionChanged" olayı tamamen SİLİNDİ.

            txtRuhsatTarihi.LostFocus += TarihKutusundanCikinca_Formatla;
            txtIskanTarihi.LostFocus += TarihKutusundanCikinca_Formatla;
        }

        private void TarihKutusundanCikinca_Formatla(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox txt && !string.IsNullOrWhiteSpace(txt.Text))
            {
                if (DateTime.TryParse(txt.Text, new System.Globalization.CultureInfo("tr-TR"), System.Globalization.DateTimeStyles.None, out DateTime dt))
                {
                    txt.Text = dt.ToString("dd.MM.yyyy");
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir tarih giriniz.\nÖrnek: 15.05.2026", "Geçersiz Tarih", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txt.Text = "";
                }
            }
        }

        private void VerileriYukle()
        {
            try
            {
                cmbRuhsatVerilis.ItemsSource = _context.RuhsatAmaclari.ToList();
                cmbRuhsatVerilis.DisplayMemberPath = "VerilisAmaci";
                cmbRuhsatVerilis.SelectedValuePath = "Id";

                cmbIskanAmaci.ItemsSource = _context.IskanAmaclari.ToList();
                cmbIskanAmaci.DisplayMemberPath = "VerilisAmaci";
                cmbIskanAmaci.SelectedValuePath = "Id";

                cmbYapiDenetim.ItemsSource = _context.Siciller.Where(x => x.SicilTipiNavigation != null && x.SicilTipiNavigation.SicilTipi == "Yapı Denetim Firması").ToList();
                cmbYapiDenetim.DisplayMemberPath = "FirmaAdi";

                cmbSantiyeSefi.ItemsSource = _context.Siciller.Where(x => x.SicilTipiNavigation != null && x.SicilTipiNavigation.SicilTipi == "Şantiye Şefi").ToList();
                cmbSantiyeSefi.DisplayMemberPath = "AdSoyad";

                cmbMuteahhit.ItemsSource = _context.Siciller.Where(x => x.SicilTipiNavigation != null && x.SicilTipiNavigation.SicilTipi == "Müteahhit").ToList();
                cmbMuteahhit.DisplayMemberPath = "FirmaAdi";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata: " + ex.Message);
            }
        }

        private WebView2 GetWbOnizleme()
        {
            return wbOnizleme;
        }

        private async void OnizlemeYukle(string yol, WebView2 wbOnizleme)
        {
            try
            {
                imgOnizleme.Visibility = Visibility.Collapsed;
                wbOnizleme.Visibility = Visibility.Collapsed;
                pnlOnizlemeUyari.Visibility = Visibility.Collapsed;

                if (string.IsNullOrEmpty(yol))
                {
                    pnlOnizlemeUyari.Visibility = Visibility.Visible;
                    return;
                }

                if (!yol.StartsWith(@"\\") && !File.Exists(yol))
                {
                    pnlOnizlemeUyari.Visibility = Visibility.Visible;
                    return;
                }

                string ext = Path.GetExtension(yol).ToLower();

                if (ext == ".pdf")
                {
                    wbOnizleme.Visibility = Visibility.Visible;
                    string webViewCacheYolu = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YapiRuhsatBelgeWebView2Cache");
                    var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, webViewCacheYolu);
                    await wbOnizleme.EnsureCoreWebView2Async(env);
                    wbOnizleme.CoreWebView2.Navigate(yol);
                }
                else if (new[] { ".jpg", ".jpeg", ".png", ".bmp" }.Contains(ext))
                {
                    imgOnizleme.Visibility = Visibility.Visible;
                    BitmapImage bitmap = new BitmapImage();
                    using (FileStream fs = new FileStream(yol, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        bitmap.BeginInit();
                        bitmap.StreamSource = fs;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                    }
                    bitmap.Freeze();
                    imgOnizleme.Source = bitmap;
                }
                else
                {
                    pnlOnizlemeUyari.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Önizleme Hatası: " + ex.Message);
                pnlOnizlemeUyari.Visibility = Visibility.Visible;
            }
        }

        private void BtnKaydet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbRuhsatVerilis.SelectedValue == null)
                {
                    MessageBox.Show("Lütfen önce bir Ruhsat Veriliş Amacı seçin!", "Uyarı");
                    return;
                }

                int secilenRuhsatAmacId = (int)cmbRuhsatVerilis.SelectedValue;
                int? secilenIskanAmacId = cmbIskanAmaci.SelectedValue != null ? (int?)cmbIskanAmaci.SelectedValue : null;

                string hedefKlasor = Path.Combine(_guvenliBelgeKlasor, _secilenYapi!.Id.ToString());
                if (!Directory.Exists(hedefKlasor)) Directory.CreateDirectory(hedefKlasor);

                string dosyaAdi = txtBelgeAdi.Text.Trim();
                if (string.IsNullOrEmpty(dosyaAdi)) return;

                // GÜNCELLEME: Aynı isimde dosya yüklenirse çakışmamaları için benzersiz (zaman damgalı) isim oluşturuluyor
                if (_mevcutBelge == null)
                {
                    string uzanti = Path.GetExtension(dosyaAdi);
                    string orijinalAd = Path.GetFileNameWithoutExtension(dosyaAdi);
                    string zamanDamgasi = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    dosyaAdi = $"{orijinalAd}_{zamanDamgasi}{uzanti}";
                }

                string hedefDosyaYolu = Path.Combine(hedefKlasor, dosyaAdi);
                string kaynakYol = txtBelgeYolu.Text.Trim();

                if (!string.IsNullOrEmpty(kaynakYol) && File.Exists(kaynakYol) && kaynakYol != hedefDosyaYolu)
                {
                    File.Copy(kaynakYol, hedefDosyaYolu, true);
                }

                string aktifKullanici = OturumYonetimi.GirisYapanKullanici?.KullaniciAdi ?? "Bilinmeyen Kullanıcı";

                // GÜNCELLEME: Artık veritabanında var mı diye kontrol edip ezmiyor. Doğrudan yeni kayıt oluşturuyor veya seçili kaydı güncelliyor.
                if (_mevcutBelge != null)
                {
                    // Düzenleme işlemi
                    var guncellenecekBelge = _context.BelgeDetay.Find(_mevcutBelge.SiraNo);
                    if (guncellenecekBelge != null)
                    {
                        guncellenecekBelge.RuhsatVerilisAmaciId = secilenRuhsatAmacId;
                        guncellenecekBelge.IskanVerilisAmaciId = secilenIskanAmacId;
                        guncellenecekBelge.RuhsatNo = txtRuhsatNo.Text;
                        guncellenecekBelge.YapiKimlikNo = txtYapiKimlikNo.Text;
                        guncellenecekBelge.RuhsatBilgileri = txtRuhsatBilgileri.Text;
                        guncellenecekBelge.RuhsatTarihi = txtRuhsatTarihi.Text;
                        guncellenecekBelge.IskanTarihi = txtIskanTarihi.Text;
                        guncellenecekBelge.BelgeyeEsasRuhsat = txtEsasRuhsat.Text;
                        guncellenecekBelge.BelgeAdi = dosyaAdi;
                        guncellenecekBelge.BelgeYolu = hedefDosyaYolu;
                        guncellenecekBelge.Aciklama = txtAciklama.Text;
                    }
                }
                else
                {
                    // Her zaman yeni belge ekleme işlemi (Sınırsız sayıda aynı tipten eklenebilir)
                    var yeniBelge = new BelgeDetayModel
                    {
                        YapiSiraNo = _secilenYapi.Id,
                        RuhsatVerilisAmaciId = secilenRuhsatAmacId,
                        IskanVerilisAmaciId = secilenIskanAmacId,
                        RuhsatNo = txtRuhsatNo.Text,
                        YapiKimlikNo = txtYapiKimlikNo.Text,
                        RuhsatBilgileri = txtRuhsatBilgileri.Text,
                        RuhsatTarihi = txtRuhsatTarihi.Text,
                        IskanTarihi = txtIskanTarihi.Text,
                        BelgeyeEsasRuhsat = txtEsasRuhsat.Text,
                        BelgeAdi = dosyaAdi,
                        BelgeYolu = hedefDosyaYolu,
                        Aciklama = txtAciklama.Text,
                        KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                        KaydedenKullanici = aktifKullanici
                    };
                    _context.BelgeDetay.Add(yeniBelge);
                }

                _context.SaveChanges();
                MessageBox.Show("Belge bilgileri başarıyla ağ arşivine kaydedildi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnKapat_Click(object sender, RoutedEventArgs e) => this.Close();

        private void BtnDosyaDegistir_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Yeni Belge Seçin",
                Filter = "Belgeler (*.pdf;*.jpg;*.png)|*.pdf;*.jpg;*.png|Tüm Dosyalar (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txtBelgeYolu.Text = openFileDialog.FileName;
                txtBelgeAdi.Text = Path.GetFileName(openFileDialog.FileName);
                OnizlemeYukle(openFileDialog.FileName, GetWbOnizleme());
            }
        }

        private void BtnBelgeListesiAc_Click(object sender, RoutedEventArgs e)
        {
            if (_secilenYapi != null && _secilenYapi.Id > 0)
            {
                BelgeListesi listePenceresi = new BelgeListesi(_secilenYapi.Id);
                listePenceresi.Owner = this;
                listePenceresi.ShowDialog();
            }
        }
    }
}