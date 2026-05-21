using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Login; // Main sayfasına erişim için
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu.KullaniciBilgileri
{
    public partial class YetkilerPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        public YetkilerPage()
        {
            InitializeComponent();
            TabloyuYenile();
        }

        private void TabloyuYenile()
        {
            // 1. Yetki Kayıtlarını Çek ve Sanal Sıra Numaralarını Ata
            var liste = _context.Yetkiler.ToList();
            for (int i = 0; i < liste.Count; i++)
            {
                liste[i].SiraNo = i + 1;
            }
            dgYetkiler.ItemsSource = liste;

            // 2. Kullanıcı Listesini Çek
            var kullanicilar = _context.Kullanicilar
                                       .Select(k => k.AdSoyad)
                                       .Where(x => x != null)
                                       .ToList();
            cmbKullanici.ItemsSource = kullanicilar;
            cmbKaydeden.ItemsSource = kullanicilar;
            cmbDegistiren.ItemsSource = kullanicilar;

            // 3. YETKİ TİPLERİ SAYFASINDAN EKLENEN VERİLERİ ÇEK
            // 'YetkiTipiAdi' alanını veritabanından alıp alfabetik sıralıyoruz
            var yetkiTipiListesi = _context.YetkiTipleri
                                           .OrderBy(x => x.YetkiTipiAdi) // Daha düzenli görünüm için sıralama ekledik
                                           .Select(x => x.YetkiTipiAdi)
                                           .Where(x => x != null)
                                           .ToList();

            cmbYetkiTipi.ItemsSource = yetkiTipiListesi;

            // 4. Modülleri Çek
            cmbModul.ItemsSource = _context.Moduller
                                           .Select(x => x.ModulAdi)
                                           .Where(x => x != null)
                                           .ToList();

            // 5. Panelleri Çek
            cmbPanel.ItemsSource = _context.Paneller
                                           .Select(x => x.PanelAdi)
                                           .Where(x => x != null)
                                           .ToList();

            // 6. Navigasyon Göstergesini Güncelle
            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgYetkiler);
            }
        }

        public void VeriyiTabloyaEkle()
        {
            if (string.IsNullOrWhiteSpace(cmbKullanici.Text) || string.IsNullOrWhiteSpace(cmbYetkiTipi.Text))
            {
                MessageBox.Show("Lütfen Kullanıcı ve Yetki Tipi seçimlerini yapın!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var yeniYetki = new YetkiModel
            {
                KullaniciAdi = cmbKullanici.Text,
                YetkiTipi = cmbYetkiTipi.Text,
                Modul = cmbModul.Text,
                Panel = cmbPanel.Text,
                YetkiVerildiMi = chbYetki.IsChecked ?? false,
                DigerBilgiler = txtDigerBilgiler.Text,
                Aciklama = txtAciklama.Text,
                KaydedenKullanici = cmbKaydeden.Text,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                DegistirenKullanici = "",
                DegistirmeTarihi = ""
            };

            _context.Yetkiler.Add(yeniYetki);
            _context.SaveChanges();
            TabloyuYenile();
            FormuTemizle();
            MessageBox.Show("Yetki başarıyla eklendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgYetkiler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgYetkiler.SelectedItem is YetkiModel secilen)
            {
                // Görsel Sıra No Basılıyor
                txtId.Text = secilen.SiraNo.ToString();

                cmbKullanici.Text = secilen.KullaniciAdi;
                cmbYetkiTipi.Text = secilen.YetkiTipi;
                cmbModul.Text = secilen.Modul;
                cmbPanel.Text = secilen.Panel;
                chbYetki.IsChecked = secilen.YetkiVerildiMi;
                txtDigerBilgiler.Text = secilen.DigerBilgiler;
                txtAciklama.Text = secilen.Aciklama;
                cmbKaydeden.Text = secilen.KaydedenKullanici;
                txtKayitTarihi.Text = secilen.KayitTarihi;
                cmbDegistiren.Text = secilen.DegistirenKullanici;
                txtDegistirmeTarihi.Text = secilen.DegistirmeTarihi;

                // Navigasyon Göstergesini Güncelle
                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgYetkiler);
                }
            }
        }

        public void VeriyiSil()
        {
            if (dgYetkiler.SelectedItem is YetkiModel secilen)
            {
                var kayit = _context.Yetkiler.Find(secilen.Id); // Gerçek ID ile bul
                if (kayit != null)
                {
                    var cevap = MessageBox.Show("Seçili yetkiyi silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (cevap == MessageBoxResult.Yes)
                    {
                        _context.Yetkiler.Remove(kayit);
                        _context.SaveChanges();
                        TabloyuYenile();
                        FormuTemizle();
                        MessageBox.Show("Yetki başarıyla silindi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz yetkiyi tablodan seçin!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void VeriyiGuncelle()
        {
            if (dgYetkiler.SelectedItem is YetkiModel secilen)
            {
                var kayit = _context.Yetkiler.Find(secilen.Id); // Gerçek ID ile bul
                if (kayit != null)
                {
                    kayit.KullaniciAdi = cmbKullanici.Text;
                    kayit.YetkiTipi = cmbYetkiTipi.Text;
                    kayit.Modul = cmbModul.Text;
                    kayit.Panel = cmbPanel.Text;
                    kayit.YetkiVerildiMi = chbYetki.IsChecked ?? false;
                    kayit.DigerBilgiler = txtDigerBilgiler.Text;
                    kayit.Aciklama = txtAciklama.Text;
                    kayit.DegistirenKullanici = cmbDegistiren.Text;
                    kayit.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    _context.SaveChanges();
                    TabloyuYenile();
                    FormuTemizle();
                    MessageBox.Show("Yetki başarıyla güncellendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz yetkiyi tablodan seçin!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void FormuTemizle()
        {
            txtId.Clear();
            cmbKullanici.SelectedIndex = -1;
            cmbYetkiTipi.SelectedIndex = -1;
            cmbModul.SelectedIndex = -1;
            cmbPanel.SelectedIndex = -1;
            chbYetki.IsChecked = false;
            txtDigerBilgiler.Clear();
            txtAciklama.Clear();
            cmbKaydeden.SelectedIndex = -1;
            txtKayitTarihi.Clear();
            cmbDegistiren.SelectedIndex = -1;
            txtDegistirmeTarihi.Clear();
            dgYetkiler.SelectedItem = null;

            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgYetkiler);
            }
        }
    }
}