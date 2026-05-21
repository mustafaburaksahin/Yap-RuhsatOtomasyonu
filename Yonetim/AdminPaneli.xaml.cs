using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Models;
using YapıRuhsatOtomasyonu.Login;

namespace YapıRuhsatOtomasyonu.Yonetim
{
    public class ModulGrupModel
    {
        public string ModulAdi { get; set; } = string.Empty;
        public List<YetkiModel> Paneller { get; set; } = new List<YetkiModel>();
    }

    public partial class AdminPaneli : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        // Arama yaparken orijinal listeyi kaybetmemek için cache listesi
        private List<KullaniciModel> tumKullanicilarHafiza = new List<KullaniciModel>();

        private readonly List<YetkiModel> TumSistemPanelleri = new List<YetkiModel>
        {
            new YetkiModel { Modul = "Yapı Bilgileri", Panel = "Yapılar" },
            new YetkiModel { Modul = "Yapı Bilgileri", Panel = "Belge Tipleri" },
            new YetkiModel { Modul = "Yapı Bilgileri", Panel = "Dosya Tipleri" },
            new YetkiModel { Modul = "Yapı Bilgileri", Panel = "Ruhsat Veriliş Amacı" },
            new YetkiModel { Modul = "Yapı Bilgileri", Panel = "İskan Veriliş Amacı" },
            new YetkiModel { Modul = "Sicil Bilgileri", Panel = "Siciller" },
            new YetkiModel { Modul = "Sicil Bilgileri", Panel = "Sicil Tipleri" },
            new YetkiModel { Modul = "Kullanıcı Bilgileri", Panel = "Kullanıcılar" },
            new YetkiModel { Modul = "Kullanıcı Bilgileri", Panel = "Ünvanlar" },
            new YetkiModel { Modul = "Kullanıcı Bilgileri", Panel = "Birimler" },
            new YetkiModel { Modul = "Yönetim", Panel = "Admin Paneli" },
            new YetkiModel { Modul = "Yönetim", Panel = "Program Ayarları" }
        };

        public AdminPaneli()
        {
            InitializeComponent();
            KullanicilariYukle();
        }

        private void KullanicilariYukle()
        {
            try
            {
                var girisYapan = OturumYonetimi.GirisYapanKullanici;
                if (girisYapan == null) return;

                string kurucuKadi = "admin";
                string kurucuSifre = "123";
                bool isKurucu = (girisYapan.KullaniciAdi == kurucuKadi && girisYapan.Sifre == kurucuSifre);

                // İsim ve TC bilgisi null olmayan, geçerli kullanıcıları çekiyoruz
                var sorgu = _context.Kullanicilar
                                    .Where(x => x.AdSoyad != null && x.TcKimlikNo != null)
                                    .AsQueryable();

                if (isKurucu)
                {
                    tumKullanicilarHafiza = sorgu.Where(x => !(x.KullaniciAdi == kurucuKadi && x.Sifre == kurucuSifre)).ToList();
                }
                else
                {
                    tumKullanicilarHafiza = sorgu.Where(x => x.KullaniciTipi == "User").ToList();
                }

                lstKullanicilar.ItemsSource = tumKullanicilarHafiza;
                if (lstKullanicilar.Items.Count == 0) icYetkiler.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı listesi yüklenemedi: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- KULLANICI ARAMA FİLTRESİ ---
        private void txtKullaniciAra_TextChanged(object sender, TextChangedEventArgs e)
        {
            string ara = txtKullaniciAra.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(ara))
            {
                lstKullanicilar.ItemsSource = tumKullanicilarHafiza;
            }
            else
            {
                var filtrelenmis = tumKullanicilarHafiza
                    .Where(x => (x.AdSoyad != null && x.AdSoyad.ToLower().Contains(ara)) ||
                                (x.TcKimlikNo != null && x.TcKimlikNo.Contains(ara)))
                    .ToList();

                lstKullanicilar.ItemsSource = filtrelenmis;
            }
        }

        private void lstKullanicilar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstKullanicilar.SelectedItem is KullaniciModel secilenUser && !string.IsNullOrEmpty(secilenUser.KullaniciAdi))
            {
                YetkileriEkranaBas(secilenUser);
            }
        }

        private void YetkileriEkranaBas(KullaniciModel secilenUser)
        {
            try
            {
                string kadi = secilenUser.KullaniciAdi ?? string.Empty;
                var dbYetkiler = _context.Yetkiler.Where(x => x.KullaniciAdi == kadi).ToList();
                var tamListe = new List<YetkiModel>();

                foreach (var sabitPanel in TumSistemPanelleri)
                {
                    if (secilenUser.KullaniciTipi == "User" &&
                       (sabitPanel.Panel == "Kullanıcılar" ||
                        sabitPanel.Panel == "Admin Paneli" ||
                        sabitPanel.Panel == "Program Ayarları"))
                    {
                        continue;
                    }

                    var mevcut = dbYetkiler.FirstOrDefault(x => x.Panel == sabitPanel.Panel);
                    tamListe.Add(mevcut ?? new YetkiModel
                    {
                        KullaniciAdi = kadi,
                        Modul = sabitPanel.Modul,
                        Panel = sabitPanel.Panel,
                        YetkiVerildiMi = false,
                        EkleYetki = false,
                        SilYetki = false,
                        GuncelleYetki = false
                    });
                }

                icYetkiler.ItemsSource = tamListe
                    .GroupBy(x => x.Modul)
                    .Select(g => new ModulGrupModel { ModulAdi = g.Key ?? "Diğer", Paneller = g.ToList() })
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Yetkiler yüklenirken hata: " + ex.Message);
            }
        }

        // --- GÖRÜNTÜLEME AÇILINCA OTO-YETKİ VERME ---
        private void Görüntüle_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton btn && btn.DataContext is YetkiModel yetki)
            {
                // KURAL: Eğer aktif edilen sayfa "Kullanıcılar" ise...
                if (yetki.Panel == "Kullanıcılar")
                {
                    yetki.EkleYetki = true;
                    yetki.SilYetki = true;
                    yetki.GuncelleYetki = true;
                    icYetkiler.Items.Refresh();
                }

                // Diğer sistem sayfaları için
                if (yetki.Panel == "Admin Paneli" || yetki.Panel == "Program Ayarları")
                {
                    yetki.EkleYetki = true;
                    yetki.SilYetki = true;
                    yetki.GuncelleYetki = true;
                    icYetkiler.Items.Refresh();
                }
            }
        }

        private void BtnTumYetkileriKaldir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ModulGrupModel grup)
            {
                foreach (var panel in grup.Paneller)
                {
                    panel.YetkiVerildiMi = false;
                    panel.EkleYetki = false;
                    panel.SilYetki = false;
                    panel.GuncelleYetki = false;
                }
                icYetkiler.Items.Refresh(); // UI'yı güncelle
            }
        }

        private void Islem_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton btn && btn.DataContext is YetkiModel yetki)
            {
                yetki.YetkiVerildiMi = true;
                icYetkiler.Items.Refresh();
            }
        }

        private void Görüntüle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton btn && btn.DataContext is YetkiModel yetki)
            {
                yetki.EkleYetki = false;
                yetki.SilYetki = false;
                yetki.GuncelleYetki = false;
                icYetkiler.Items.Refresh();
            }
        }

        private void BtnYetkiKaydet_Click(object sender, RoutedEventArgs e)
        {
            if (lstKullanicilar.SelectedItem is KullaniciModel secilenUser)
            {
                try
                {
                    if (!(icYetkiler.ItemsSource is List<ModulGrupModel> gruplar)) return;

                    foreach (var grup in gruplar)
                    {
                        foreach (var satir in grup.Paneller)
                        {
                            var dbKayit = _context.Yetkiler.FirstOrDefault(x => x.KullaniciAdi == secilenUser.KullaniciAdi && x.Panel == satir.Panel);

                            if (dbKayit != null)
                            {
                                dbKayit.YetkiVerildiMi = satir.YetkiVerildiMi;
                                dbKayit.EkleYetki = satir.EkleYetki;
                                dbKayit.SilYetki = satir.SilYetki;
                                dbKayit.GuncelleYetki = satir.GuncelleYetki;
                            }
                            else if (satir.YetkiVerildiMi || satir.EkleYetki || satir.SilYetki || satir.GuncelleYetki)
                            {
                                satir.KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                                // DÜZELTME: Loglama sistemine uygun olarak KullaniciAdi kaydediliyor
                                satir.KaydedenKullanici = OturumYonetimi.GirisYapanKullanici?.KullaniciAdi;
                                _context.Yetkiler.Add(satir);
                            }
                        }
                    }
                    _context.SaveChanges();
                    MessageBox.Show($"{secilenUser.AdSoyad} için yetkiler başarıyla kaydedildi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                    YetkileriEkranaBas(secilenUser);
                }
                catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            }
        }

        private void BtnTumYetkileriVer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ModulGrupModel grup)
            {
                foreach (var panel in grup.Paneller)
                {
                    panel.YetkiVerildiMi = panel.EkleYetki = panel.SilYetki = panel.GuncelleYetki = true;
                }
                icYetkiler.Items.Refresh();
            }
        }
    }
}