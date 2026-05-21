using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection; // Reflection eklendi (Dinamik özellik okuma için)
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Login;
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu.KullaniciBilgileri
{
    public partial class KullanicilarPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        string aktifKullanici = OturumYonetimi.GirisYapanKullanici?.AdSoyad ?? "Bilinmeyen Kullanıcı";
        private bool IsSuperAdmin => OturumYonetimi.GirisYapanKullanici?.KullaniciAdi == "admin";

        // Filtreleme için tanımlar
        private ICollectionView _kullanicilarView = null!;
        private Dictionary<string, string> _filtreler = new Dictionary<string, string>();

        public KullanicilarPage()
        {
            InitializeComponent();

            if (OturumYonetimi.GirisYapanKullanici?.KullaniciTipi != "Admin")
            {
                MessageBox.Show("Bu sayfaya erişim yetkiniz bulunmamaktadır!", "Yetki Engeli", MessageBoxButton.OK, MessageBoxImage.Stop);
                this.IsEnabled = false;
                return;
            }

            cmbKullaniciTipi.Items.Clear();
            if (IsSuperAdmin) { cmbKullaniciTipi.Items.Add("Admin"); cmbKullaniciTipi.Items.Add("User"); }
            else { cmbKullaniciTipi.Items.Add("User"); }

            txtAnneAdi.PreviewTextInput += SadeceHarf_PreviewTextInput;
            txtBabaAdi.PreviewTextInput += SadeceHarf_PreviewTextInput;
            txtAdSoyad.PreviewTextInput += SadeceHarf_PreviewTextInput;

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

        private void SadeceHarf_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        private void TabloyuYenile()
        {
            try
            {
                var girisYapan = OturumYonetimi.GirisYapanKullanici;
                if (girisYapan == null) return;

                var sorgu = _context.Kullanicilar.Include(x => x.Unvan).Include(x => x.Birim).Where(x => x.KullaniciAdi != "admin");
                if (!IsSuperAdmin) sorgu = sorgu.Where(x => x.KullaniciTipi == "User" && x.Id != girisYapan.Id);

                var liste = sorgu.ToList();
                for (int i = 0; i < liste.Count; i++) { liste[i].SiraNo = i + 1; }

                // Filtreleme mimarisi bağlaması
                _kullanicilarView = CollectionViewSource.GetDefaultView(liste);
                _kullanicilarView.Filter = KullaniciFiltreMantigi;
                dgKullanicilar.ItemsSource = _kullanicilarView;

                cmbUnvan.ItemsSource = _context.Unvanlar.ToList();
                cmbUnvan.DisplayMemberPath = "UnvanAdi"; cmbUnvan.SelectedValuePath = "Id";
                cmbBirim.ItemsSource = _context.Birimler.ToList();
                cmbBirim.DisplayMemberPath = "BirimAdi"; cmbBirim.SelectedValuePath = "Id";

                FormuTemizle();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        // METİN (TextBox) Sütunları İçin Filtre Tetikleyici
        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb && tb.Tag != null)
            {
                _filtreler[tb.Tag.ToString()!] = tb.Text.Trim();
                _kullanicilarView?.Refresh();
            }
        }

        // AKTİF/PASİF (ComboBox) Sütunları İçin Filtre Tetikleyici
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cb && cb.Tag != null && cb.SelectedItem is ComboBoxItem item)
            {
                string column = cb.Tag.ToString()!;
                string deger = item.Content.ToString()!;

                // "Tümü" seçildiyse filtreyi temizle, değilse değeri ata
                _filtreler[column] = deger == "Tümü" ? string.Empty : deger;
                _kullanicilarView?.Refresh();
            }
        }

        // DİNAMİK FİLTRELEME MANTIĞI
        private bool KullaniciFiltreMantigi(object item)
        {
            if (!(item is KullaniciModel k)) return false;

            foreach (var filter in _filtreler)
            {
                // Kutucuk boşsa o sütunu filtreleme
                if (string.IsNullOrWhiteSpace(filter.Value)) continue;

                string columnName = filter.Key;
                string searchValue = filter.Value.ToLower(); // Kolay eşleşme için küçük harfe çevir

                // Tag ile belirtilen özelliği Reflection ile bul
                object? propertyValue = GetPropertyValue(k, columnName);

                // Eğer özellik o satırda null ise ve biz bir şey arıyorsak, eşleşmedi demektir
                if (propertyValue == null) return false;

                // Checkbox (AktifMi) özel kontrolü
                if (columnName == "AktifMi")
                {
                    bool isAktif = (bool)propertyValue;
                    if (filter.Value == "Aktif" && !isAktif) return false;
                    if (filter.Value == "Pasif" && isAktif) return false;
                    continue;
                }

                // Standart string kontrolü
                if (!propertyValue.ToString()!.ToLower().Contains(searchValue))
                {
                    return false; // İçinde geçmiyorsa bu satırı gizle
                }
            }

            return true; // Tüm filtre şartlarını sağlıyorsa göster
        }

        // Unvan.UnvanAdi gibi iç içe propları okuyan yardımcı metot
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

        public void VeriyiTabloyaEkle()
        {
            var yeniKullanici = new KullaniciModel
            {
                AktifMi = chkAktifMi.IsChecked ?? false,
                TcKimlikNo = txtTcKimlikNo.Text,
                AdSoyad = txtAdSoyad.Text,
                KullaniciAdi = txtKullaniciAdi.Text,
                Sifre = txtSifre.Text,
                KullaniciTipi = cmbKullaniciTipi.Text,
                UnvanId = (int?)cmbUnvan.SelectedValue,
                BirimId = (int?)cmbBirim.SelectedValue,
                KaydedenKullanici = aktifKullanici,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
            };

            if (!KullaniciDogrula(yeniKullanici, "Ekle")) return;
            _context.Kullanicilar.Add(yeniKullanici);
            _context.SaveChanges();
            TabloyuYenile();
            MessageBox.Show("Kayıt başarıyla tamamlandı.");
        }

        public void VeriyiSil()
        {
            if (dgKullanicilar.SelectedItem is KullaniciModel s)
            {
                if (MessageBox.Show("Silinsin mi?", "Onay", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _context.Kullanicilar.Remove(s);
                    _context.SaveChanges();
                    TabloyuYenile();
                }
            }
        }

        public void VeriyiGuncelle()
        {
            if (dgKullanicilar.SelectedItem is KullaniciModel s)
            {
                var g = _context.Kullanicilar.Find(s.Id);
                if (g != null)
                {
                    g.AdSoyad = txtAdSoyad.Text;
                    g.KullaniciAdi = txtKullaniciAdi.Text;
                    g.DegistirenKullanici = aktifKullanici;
                    g.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                    _context.SaveChanges();
                    TabloyuYenile();
                    MessageBox.Show("Güncellendi.");
                }
            }
        }

        public void FormuTemizle()
        {
            txtId.Clear(); txtAdSoyad.Clear(); txtTcKimlikNo.Clear();
            txtKaydeden.Clear(); txtKayitTarihi.Clear();
            txtDegistiren.Clear(); if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Clear();
            dgKullanicilar.SelectedIndex = -1;
        }

        private void dgKullanicilar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgKullanicilar.SelectedItem is KullaniciModel s)
            {
                txtId.Text = s.SiraNo.ToString();
                txtAdSoyad.Text = s.AdSoyad;
                txtTcKimlikNo.Text = s.TcKimlikNo;
                txtKaydeden.Text = s.KaydedenKullanici;
                txtKayitTarihi.Text = s.KayitTarihi;
                txtDegistiren.Text = s.DegistirenKullanici;
                if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = s.DegistirmeTarihi;
            }
        }

        private bool KullaniciDogrula(KullaniciModel k, string islem) => true; // Basitleştirilmiş
    }
}