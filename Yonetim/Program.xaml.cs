using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu.Yonetim
{
    public partial class Program : Page
    {
        private string secilenLogoYolu = "";

        // GÜNCELLEME: Logoyu tüm bilgisayarların görebilmesi için ortak ağ klasörüne yönlendirdik
        // Bilgisayar adı yerine doğrudan ana makinenin sabit yerel IP adresine bağladık
        private readonly string _guvenliAyarKlasor = @"C:\YapiRuhsatArsiv\Ayarlar";

        public Program()
        {
            InitializeComponent();
            MevcutAyarlariGetir(); // Sayfa açılırken DB'den verileri çeker
        }

        // 1. VERİTABANINDAN MEVCUT AYARLARI OKUMA
        private void MevcutAyarlariGetir()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var ayar = context.SistemAyarlari.FirstOrDefault();
                    if (ayar != null)
                    {
                        txtSistemAdi.Text = ayar.SistemAdi;
                        txtBelediyeAdi.Text = ayar.BelediyeAdi;
                        txtIl.Text = ayar.Il;
                        txtIlce.Text = ayar.Ilce;

                        // GÜNCELLEME: Logoyu DB'den çekip ekrana basma (varsa)
                        if (!string.IsNullOrEmpty(ayar.LogoYolu) && File.Exists(ayar.LogoYolu))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            using (FileStream fs = new FileStream(ayar.LogoYolu, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                bitmap.BeginInit();
                                bitmap.StreamSource = fs;
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();
                            }
                            bitmap.Freeze();
                            imgLogoOnizleme.Source = bitmap;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // İlk açılışta veritabanı boşsa hata vermemesi için catch bloğu eklendi
            }
        }

        // 2. LOGO SEÇME İŞLEMİ
        private void BtnLogoSec_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Belediye Logosu Seç";
            openFileDialog.Filter = "Resim Dosyaları (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (openFileDialog.ShowDialog() == true)
            {
                secilenLogoYolu = openFileDialog.FileName;

                // Dosyayı kilitlemeden önizleme yap
                BitmapImage bitmap = new BitmapImage();
                using (FileStream fs = new FileStream(secilenLogoYolu, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = fs;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
                bitmap.Freeze();
                imgLogoOnizleme.Source = bitmap;
            }
        }

        // 3. AYARLARI VERİTABANINA KAYDETME VE CANLI GÜNCELLEME
        private void BtnAyarlariKaydet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    // DB'deki ilk kaydı al, yoksa yeni oluştur
                    var ayar = context.SistemAyarlari.FirstOrDefault();
                    if (ayar == null)
                    {
                        ayar = new SistemAyarModel();
                        context.SistemAyarlari.Add(ayar);
                    }

                    // Ekrandaki güncel bilgileri başındaki/sonundaki boşlukları silerek (Trim) Modele at
                    ayar.SistemAdi = txtSistemAdi.Text.Trim();
                    ayar.BelediyeAdi = txtBelediyeAdi.Text.Trim();
                    ayar.Il = txtIl.Text.Trim();
                    ayar.Ilce = txtIlce.Text.Trim();

                    // GÜNCELLEME: Logoyu yerel bilgisayardan alıp, herkesin görebileceği ortak ağ klasörüne kopyala
                    if (!string.IsNullOrEmpty(secilenLogoYolu) && File.Exists(secilenLogoYolu))
                    {
                        // Ayarlar klasörünü kontrol et, yoksa oluştur
                        if (!Directory.Exists(_guvenliAyarKlasor)) Directory.CreateDirectory(_guvenliAyarKlasor);

                        string uzanti = Path.GetExtension(secilenLogoYolu);
                        string yeniLogoAdi = $"BelediyeLogosu_{DateTime.Now:yyyyMMdd_HHmmss}{uzanti}";
                        string hedefTamYol = Path.Combine(_guvenliAyarKlasor, yeniLogoAdi);

                        // Eğer seçilen resim zaten arşivin içinde değilse taşı (aynı dosyayı seçme hatasını engeller)
                        if (secilenLogoYolu != hedefTamYol)
                        {
                            File.Copy(secilenLogoYolu, hedefTamYol, true);
                            ayar.LogoYolu = hedefTamYol; // Veritabanına ağ yolunu kaydet
                        }
                    }

                    // Veritabanına kaydet
                    context.SaveChanges();
                }

                // --- CANLI GÜNCELLEME ---
                // Kayıt başarılı olduktan sonra Main penceresinin başlığını anında güncelle
                if (Window.GetWindow(this) is YapıRuhsatOtomasyonu.Login.Main anaPencere)
                {
                    anaPencere.BasligiGuncelle();
                }

                MessageBox.Show("Ayarlar veritabanına başarıyla kaydedildi!", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ayarlar kaydedilirken hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- METİN KUTULARINDAN ÇIKILINCA BOŞLUKLARI OTOMATİK SİLEN METOT ---
        private void TextBox_BoslukTemizle(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox txt && !string.IsNullOrEmpty(txt.Text))
            {
                txt.Text = txt.Text.Trim();
            }
        }
    }
}