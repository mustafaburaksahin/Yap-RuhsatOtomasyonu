using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection; // Dinamik filtreleme için eklendi
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data; // ICollectionView ve CollectionViewSource için eklendi
using Microsoft.EntityFrameworkCore;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Models;
using YapıRuhsatOtomasyonu.Login;

namespace YapıRuhsatOtomasyonu.SicilBilgileri
{
    public partial class SicilPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        string aktifKullanici = OturumYonetimi.GirisYapanKullanici?.AdSoyad ?? "Bilinmeyen Kullanıcı";

        // FİLTRELEME İÇİN EKLENEN TANIMLAR
        private ICollectionView _sicillerView = null!;
        private Dictionary<string, string> _filtreler = new Dictionary<string, string>();

        public SicilPage()
        {
            InitializeComponent();
            TabloyuYenile();

            this.Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = OturumYonetimi.GirisYapanKullanici;
            if (user == null || string.IsNullOrEmpty(user.KullaniciAdi)) return;

            if (user.KullaniciAdi == "admin")
            {
                if (Panel1 != null) Panel1.Visibility = Visibility.Visible;
                return;
            }

            var yetki = _context.Yetkiler.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi && (x.Panel == "Siciller" || x.Panel == "Sicil"));
            bool islemYetkisiVar = yetki != null && (yetki.EkleYetki || yetki.GuncelleYetki);

            if (Panel1 != null) Panel1.Visibility = islemYetkisiVar ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TabloyuYenile()
        {
            try
            {
                var tumVeriler = _context.Siciller
                                         .Include(s => s.SicilTipiNavigation)
                                         .ToList();

                var formatliListe = tumVeriler
                    .GroupBy(s => s.TcKimlikNo)
                    .Select((g, index) => new
                    {
                        SiraNo = index + 1,
                        Id = g.First().Id,
                        AdSoyad = g.First().AdSoyad,
                        TcKimlikNo = g.Key,
                        VergiNo = g.First().VergiNo,
                        FirmaAdi = g.First().FirmaAdi,
                        DogumYeri = g.First().DogumYeri,
                        DogumTarihi = g.First().DogumTarihi,
                        VergiDairesi = g.First().VergiDairesi,
                        AnneAdi = g.First().AnneAdi,
                        BabaAdi = g.First().BabaAdi,
                        TelefonNo = g.First().TelefonNo,
                        Email = g.First().Email,
                        AlanKisi = g.First().AlanKisi,
                        Adres = g.First().Adres,
                        Aciklama = g.First().Aciklama,
                        DigerBilgiler = g.First().DigerBilgiler,
                        KaydedenKullanici = g.First().KaydedenKullanici,
                        KayitTarihi = g.First().KayitTarihi,
                        DegistirenKullanici = g.First().DegistirenKullanici,
                        DegistirmeTarihi = g.First().DegistirmeTarihi,
                        SicilTipi = string.Join(", ", g.Where(x => x.SicilTipiNavigation != null)
                                                       .Select(x => x.SicilTipiNavigation?.SicilTipi)
                                                       .Distinct())
                    })
                    .ToList();

                // FİLTRELEME BAĞLANTISI EKLENDİ (ItemsSource yerine CollectionView bağlandı)
                _sicillerView = CollectionViewSource.GetDefaultView(formatliListe);
                _sicillerView.Filter = SicilFiltreMantigi;
                dgSiciller.ItemsSource = _sicillerView;

                cmbSicilTipi.ItemsSource = _context.SicilTipleri.ToList();
                cmbSicilTipi.DisplayMemberPath = "SicilTipi";
                cmbSicilTipi.SelectedValuePath = "Id";

                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgSiciller);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- FİLTRELEME METOTLARI (EKLENDİ) ---

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb && tb.Tag != null)
            {
                _filtreler[tb.Tag.ToString()!] = tb.Text.Trim();
                _sicillerView?.Refresh();
            }
        }

        private bool SicilFiltreMantigi(object item)
        {
            if (item == null) return false;

            foreach (var filter in _filtreler)
            {
                // Boş filtre alanlarını es geç
                if (string.IsNullOrWhiteSpace(filter.Value)) continue;

                string columnName = filter.Key;
                string searchValue = filter.Value.ToLower();

                // Reflection ile isimsiz tipten (anonymous type) değer okunuyor
                object? propertyValue = GetPropertyValue(item, columnName);

                if (propertyValue == null) return false;

                // İçeriyorsa devam et, içermiyorsa kaydı gizle
                if (!propertyValue.ToString()!.ToLower().Contains(searchValue))
                {
                    return false;
                }
            }
            return true;
        }

        private object? GetPropertyValue(object obj, string propertyName)
        {
            foreach (var prop in propertyName.Split('.'))
            {
                if (obj == null) return null;
                PropertyInfo? pi = obj.GetType().GetProperty(prop);
                if (pi == null) return null;
                obj = pi.GetValue(obj, null)!;
            }
            return obj;
        }

        // --- CRUD İŞLEMLERİ (Aynı Bırakıldı) ---

        private bool GirdiDogrula()
        {
            if (string.IsNullOrWhiteSpace(txtTcKimlikNo.Text) ||
                cmbSicilTipi.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtAdSoyad.Text) ||
                string.IsNullOrWhiteSpace(txtVergiNo.Text) ||
                string.IsNullOrWhiteSpace(txtTelefonNo.Text) ||
                string.IsNullOrWhiteSpace(txtAdres.Text) ||
                string.IsNullOrWhiteSpace(txtAlanKisi.Text))
            {
                MessageBox.Show("Zorunlu alanları doldurun!", "Eksik Bilgi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            string tc = txtTcKimlikNo.Text.Trim();
            if (tc.Length != 11 || !tc.All(char.IsDigit))
            {
                MessageBox.Show("TC Kimlik No tam 11 haneli ve rakam olmalı!", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public void VeriyiTabloyaEkle()
        {
            if (!GirdiDogrula()) return;

            string girilenTc = txtTcKimlikNo.Text.Trim();
            int secilenTipId = (int)cmbSicilTipi.SelectedValue;

            if (_context.Siciller.Any(s => s.TcKimlikNo == girilenTc && s.SicilTipi == secilenTipId))
            {
                MessageBox.Show("Bu sicil kaydı zaten mevcut!", "Mükerrer Kayıt", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var yeniSicil = new SicilModel
            {
                SicilTipi = secilenTipId,
                TcKimlikNo = girilenTc,
                VergiNo = txtVergiNo.Text,
                FirmaAdi = txtFirmaAdi.Text,
                AdSoyad = txtAdSoyad.Text,
                DogumYeri = txtDogumYeri.Text,
                DogumTarihi = dpDogumTarihi.Text,
                VergiDairesi = txtVergiDairesi.Text,
                AnneAdi = txtAnneAdi.Text,
                BabaAdi = txtBabaAdi.Text,
                TelefonNo = txtTelefonNo.Text,
                Email = txtEmail.Text,
                AlanKisi = txtAlanKisi.Text,
                Adres = txtAdres.Text,
                Aciklama = txtAciklama.Text,
                DigerBilgiler = txtDigerBilgiler.Text,

                KaydedenKullanici = aktifKullanici,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
            };

            _context.Siciller.Add(yeniSicil);
            _context.SaveChanges();

            TabloyuYenile();
            FormuTemizle();
            MessageBox.Show("Kayıt başarıyla eklendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgSiciller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSiciller.SelectedItem != null)
            {
                dynamic secilen = dgSiciller.SelectedItem;

                txtId.Text = secilen.SiraNo.ToString();

                string? ilkUnvanAdi = (secilen.SicilTipi as string)?.Split(',')[0].Trim();
                var tipModel = _context.SicilTipleri.FirstOrDefault(t => t.SicilTipi == ilkUnvanAdi);
                if (tipModel != null)
                {
                    cmbSicilTipi.SelectedValue = tipModel.Id;
                }

                txtTcKimlikNo.Text = secilen.TcKimlikNo;
                txtVergiNo.Text = secilen.VergiNo;
                txtFirmaAdi.Text = secilen.FirmaAdi;
                txtAdSoyad.Text = secilen.AdSoyad;
                txtDogumYeri.Text = secilen.DogumYeri;
                dpDogumTarihi.Text = secilen.DogumTarihi;
                txtVergiDairesi.Text = secilen.VergiDairesi;
                txtAnneAdi.Text = secilen.AnneAdi;
                txtBabaAdi.Text = secilen.BabaAdi;
                txtTelefonNo.Text = secilen.TelefonNo;
                txtEmail.Text = secilen.Email;
                txtAlanKisi.Text = secilen.AlanKisi;
                txtAdres.Text = secilen.Adres;
                txtAciklama.Text = secilen.Aciklama;
                txtDigerBilgiler.Text = secilen.DigerBilgiler;

                txtKaydeden.Text = secilen.KaydedenKullanici;
                txtKayitTarihi.Text = secilen.KayitTarihi;
                txtDegistiren.Text = secilen.DegistirenKullanici;

                if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = secilen.DegistirmeTarihi;

                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgSiciller);
                }
            }
        }

        public void VeriyiSil()
        {
            if (dgSiciller.SelectedItem != null)
            {
                // Silmek için önce formda bir sicil tipinin seçili olup olmadığını kontrol edelim
                if (cmbSicilTipi.SelectedValue == null)
                {
                    MessageBox.Show("Silmek istediğiniz sicil tipini yukarıdan (Sicil Tipi ComboBox'ından) seçin.", "Eksik Seçim", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                dynamic secilen = dgSiciller.SelectedItem;
                string tcNo = secilen.TcKimlikNo;
                int secilenTipId = (int)cmbSicilTipi.SelectedValue;
                string secilenTipAdi = cmbSicilTipi.Text;

                if (MessageBox.Show($"Bu kişiye ait '{secilenTipAdi}' sicil kaydını silmek istediğinize emin misiniz?", "Kısmi Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // Sadece o TC'ye ve o Sicil Tipine ait olan spesifik kaydı bul
                    var silinecekKayit = _context.Siciller.FirstOrDefault(x => x.TcKimlikNo == tcNo && x.SicilTipi == secilenTipId);

                    if (silinecekKayit != null)
                    {
                        _context.Siciller.Remove(silinecekKayit);
                        _context.SaveChanges();

                        TabloyuYenile();
                        FormuTemizle();
                        MessageBox.Show("Seçilen sicil tipi başarıyla silindi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Bu kişiye ait böyle bir sicil tipi bulunamadı! Lütfen kişinin sahip olduğu sicil tiplerinden birini seçin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void VeriyiGuncelle()
        {
            if (dgSiciller.SelectedItem != null)
            {
                if (!GirdiDogrula()) return;

                dynamic secilen = dgSiciller.SelectedItem;
                string eskiTc = secilen.TcKimlikNo;

                // O kişiye ait TÜM sicil satırlarını getir (çünkü isim, adres vs değişirse hepsinde değişmeli)
                var guncellenecekKayitlar = _context.Siciller.Where(x => x.TcKimlikNo == eskiTc).ToList();

                if (guncellenecekKayitlar.Any())
                {
                    string guncelZaman = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    // Formdaki yeni TC numarasını al
                    string yeniTc = txtTcKimlikNo.Text.Trim();

                    // Eğer kullanıcı TC numarasını değiştiriyorsa ve yeni TC başkasına aitse engelle
                    if (eskiTc != yeniTc && _context.Siciller.Any(x => x.TcKimlikNo == yeniTc))
                    {
                        MessageBox.Show("Girdiğiniz yeni TC Kimlik numarası başka bir sicil kaydına ait!", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // TÜM KAYITLARIN ortak bilgilerini (Adres, İsim, TC vs.) güncelle
                    foreach (var kayit in guncellenecekKayitlar)
                    {
                        // DİKKAT: kayit.SicilTipi = secilenTipId; KISMINI KALDIRDIK. 
                        // Sicil tipleri kendi hallerinde kalmaya devam edecek.

                        kayit.TcKimlikNo = yeniTc;
                        kayit.VergiNo = txtVergiNo.Text;
                        kayit.FirmaAdi = txtFirmaAdi.Text;
                        kayit.AdSoyad = txtAdSoyad.Text;
                        kayit.DogumYeri = txtDogumYeri.Text;
                        kayit.DogumTarihi = dpDogumTarihi.Text;
                        kayit.VergiDairesi = txtVergiDairesi.Text;
                        kayit.AnneAdi = txtAnneAdi.Text;
                        kayit.BabaAdi = txtBabaAdi.Text;
                        kayit.TelefonNo = txtTelefonNo.Text;
                        kayit.Email = txtEmail.Text;
                        kayit.AlanKisi = txtAlanKisi.Text;
                        kayit.Adres = txtAdres.Text;
                        kayit.Aciklama = txtAciklama.Text;
                        kayit.DigerBilgiler = txtDigerBilgiler.Text;

                        kayit.DegistirenKullanici = aktifKullanici;
                        kayit.DegistirmeTarihi = guncelZaman;
                    }

                    _context.SaveChanges();

                    txtDegistiren.Text = aktifKullanici;
                    if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = guncelZaman;

                    TabloyuYenile();
                    MessageBox.Show("Kişinin bilgileri başarıyla güncellendi (Mevcut sicil tipleri korundu).", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    FormuTemizle();
                }
            }
        }

        public void FormuTemizle()
        {
            txtId.Text = string.Empty;
            cmbSicilTipi.SelectedIndex = -1;
            txtTcKimlikNo.Clear();
            txtVergiNo.Clear();
            txtFirmaAdi.Clear();
            txtAdSoyad.Clear();
            txtDogumYeri.Clear();
            dpDogumTarihi.SelectedDate = null;
            txtVergiDairesi.Clear();
            txtAnneAdi.Clear();
            txtBabaAdi.Clear();
            txtTelefonNo.Clear();
            txtEmail.Clear();
            txtAlanKisi.Clear();
            txtAdres.Clear();
            txtAciklama.Clear();
            txtDigerBilgiler.Clear();

            txtKaydeden.Clear();
            txtKayitTarihi.Clear();
            txtDegistiren.Clear();
            if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Clear();

            dgSiciller.SelectedItem = null;

            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgSiciller);
            }
        }
    }
}