using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Login; // Main sayfasına erişim için eklendi
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu.Yonetim
{
    public partial class Paneller : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        public Paneller()
        {
            InitializeComponent();
            TabloyuYenile();
        }

        private void TabloyuYenile()
        {
            // 1. Verileri veritabanından çek
            var liste = _context.Paneller.ToList();

            // 2. Sanal Sıra Numaralarını ata (Görsel kaymayı engeller)
            for (int i = 0; i < liste.Count; i++)
            {
                liste[i].SiraNo = i + 1;
            }

            dgPaneller.ItemsSource = liste;

            // 3. Kullanıcıları ComboBox'lara doldur
            var kullanicilar = _context.Kullanicilar.Select(k => k.AdSoyad).ToList();
            cmbKaydeden.ItemsSource = kullanicilar;
            cmbDegistiren.ItemsSource = kullanicilar;

            // 4. Modülleri ComboBox'a doldur
            cmbModul.ItemsSource = _context.Moduller.Select(m => m.ModulAdi).ToList();

            // --- NAVİGASYON GÜNCELLEME ---
            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgPaneller);
            }
        }

        public void VeriyiTabloyaEkle()
        {
            if (string.IsNullOrWhiteSpace(txtPanelAdi.Text))
            {
                MessageBox.Show("Lütfen Panel Adı alanını doldurun!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var yeniPanel = new PanelModel
            {
                PanelAdi = txtPanelAdi.Text,
                ModulAdi = cmbModul.Text,
                DigerBilgiler = txtDigerBilgiler.Text,
                Aciklama = txtAciklama.Text,

                // Log Alanları
                KaydedenKullanici = cmbKaydeden.Text,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                DegistirenKullanici = "",
                DegistirmeTarihi = ""
            };

            _context.Paneller.Add(yeniPanel);
            _context.SaveChanges();

            TabloyuYenile();
            FormuTemizle();
            MessageBox.Show("Panel başarıyla eklendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgPaneller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPaneller.SelectedItem is PanelModel secilen)
            {
                // Görsel Sıra No Basılıyor
                txtId.Text = secilen.SiraNo.ToString();

                txtPanelAdi.Text = secilen.PanelAdi;
                cmbModul.Text = secilen.ModulAdi;
                txtDigerBilgiler.Text = secilen.DigerBilgiler;
                txtAciklama.Text = secilen.Aciklama;

                // Log Alanlarını Doldur
                cmbKaydeden.Text = secilen.KaydedenKullanici;
                txtKayitTarihi.Text = secilen.KayitTarihi;
                cmbDegistiren.Text = secilen.DegistirenKullanici;
                txtDegistirmeTarihi.Text = secilen.DegistirmeTarihi;

                // --- NAVİGASYON GÜNCELLEME ---
                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgPaneller);
                }
            }
        }

        public void VeriyiSil()
        {
            if (dgPaneller.SelectedItem is PanelModel secilen)
            {
                var sonuc = MessageBox.Show("Seçili paneli silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (sonuc == MessageBoxResult.Yes)
                {
                    // Silme işlemi GERÇEK ID üzerinden yapılır
                    var kayit = _context.Paneller.Find(secilen.Id);
                    if (kayit != null)
                    {
                        _context.Paneller.Remove(kayit);
                        _context.SaveChanges();

                        TabloyuYenile();
                        FormuTemizle();
                        MessageBox.Show("Panel başarıyla silindi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için tablodan bir panel seçin!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void VeriyiGuncelle()
        {
            if (dgPaneller.SelectedItem is PanelModel secilen)
            {
                // Güncelleme işlemi GERÇEK ID üzerinden yapılır
                var kayit = _context.Paneller.Find(secilen.Id);
                if (kayit != null)
                {
                    kayit.PanelAdi = txtPanelAdi.Text;
                    kayit.ModulAdi = cmbModul.Text;
                    kayit.DigerBilgiler = txtDigerBilgiler.Text;
                    kayit.Aciklama = txtAciklama.Text;

                    // Log güncellemesi
                    kayit.DegistirenKullanici = cmbDegistiren.Text;
                    kayit.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    _context.SaveChanges();
                    TabloyuYenile();
                    FormuTemizle();
                    MessageBox.Show("Panel başarıyla güncellendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek için tablodan bir panel seçin!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void FormuTemizle()
        {
            txtId.Clear();
            txtPanelAdi.Clear();
            cmbModul.SelectedIndex = -1;
            txtDigerBilgiler.Clear();
            txtAciklama.Clear();

            cmbKaydeden.SelectedIndex = -1;
            txtKayitTarihi.Clear();
            cmbDegistiren.SelectedIndex = -1;
            txtDegistirmeTarihi.Clear();

            dgPaneller.SelectedItem = null;

            // Navigasyonu temizleme sonrası güncelle
            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgPaneller);
            }
        }
    }
}