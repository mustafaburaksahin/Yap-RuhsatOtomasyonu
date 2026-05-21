using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Login; // Main sayfasına erişim için eklendi
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu.Yonetim
{
    public partial class Moduller : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        public Moduller()
        {
            InitializeComponent();
            TabloyuYenile();
        }

        private void TabloyuYenile()
        {
            // 1. Verileri veritabanından çek
            var liste = _context.Moduller.ToList();

            // 2. Sanal Sıra Numaralarını ata (Görsel kaymayı engeller)
            for (int i = 0; i < liste.Count; i++)
            {
                liste[i].SiraNo = i + 1;
            }

            dgModuller.ItemsSource = liste;

            // 3. Kullanıcıları ComboBox'lara doldur
            var kullanicilar = _context.Kullanicilar.Select(k => k.AdSoyad).ToList();
            cmbKaydeden.ItemsSource = kullanicilar;
            cmbDegistiren.ItemsSource = kullanicilar;

            // --- NAVİGASYON GÜNCELLEME ---
            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgModuller);
            }
        }

        public void VeriyiTabloyaEkle()
        {
            if (string.IsNullOrWhiteSpace(txtModulAdi.Text))
            {
                MessageBox.Show("Lütfen Modül Adı alanını doldurun!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var yeniModul = new ModulModel
            {
                ModulAdi = txtModulAdi.Text,
                DigerBilgiler = txtDigerBilgiler.Text,
                Aciklama = txtAciklama.Text,

                // Log Alanları
                KaydedenKullanici = cmbKaydeden.Text,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                DegistirenKullanici = "",
                DegistirmeTarihi = ""
            };

            _context.Moduller.Add(yeniModul);
            _context.SaveChanges();

            TabloyuYenile();
            FormuTemizle();
            MessageBox.Show("Modül başarıyla eklendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgModuller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgModuller.SelectedItem is ModulModel secilen)
            {
                // Görsel Sıra No Basılıyor
                txtId.Text = secilen.SiraNo.ToString();

                txtModulAdi.Text = secilen.ModulAdi;
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
                    anaPencere.UpdateNavDisplay(dgModuller);
                }
            }
        }

        public void VeriyiSil()
        {
            if (dgModuller.SelectedItem is ModulModel secilen)
            {
                var sonuc = MessageBox.Show("Seçili modülü silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (sonuc == MessageBoxResult.Yes)
                {
                    // Silme işlemi GERÇEK ID üzerinden yapılır
                    var kayit = _context.Moduller.Find(secilen.Id);
                    if (kayit != null)
                    {
                        _context.Moduller.Remove(kayit);
                        _context.SaveChanges();

                        TabloyuYenile();
                        FormuTemizle();
                        MessageBox.Show("Modül başarıyla silindi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için tablodan bir modül seçin!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void VeriyiGuncelle()
        {
            if (dgModuller.SelectedItem is ModulModel secilen)
            {
                // Güncelleme işlemi GERÇEK ID üzerinden yapılır
                var kayit = _context.Moduller.Find(secilen.Id);
                if (kayit != null)
                {
                    kayit.ModulAdi = txtModulAdi.Text;
                    kayit.DigerBilgiler = txtDigerBilgiler.Text;
                    kayit.Aciklama = txtAciklama.Text;

                    // Log güncellemesi
                    kayit.DegistirenKullanici = cmbDegistiren.Text;
                    kayit.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    _context.SaveChanges();
                    TabloyuYenile();
                    FormuTemizle();
                    MessageBox.Show("Modül başarıyla güncellendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek için tablodan bir modül seçin!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void FormuTemizle()
        {
            txtId.Clear();
            txtModulAdi.Clear();
            txtDigerBilgiler.Clear();
            txtAciklama.Clear();

            cmbKaydeden.SelectedIndex = -1;
            txtKayitTarihi.Clear();
            cmbDegistiren.SelectedIndex = -1;
            txtDegistirmeTarihi.Clear();

            dgModuller.SelectedItem = null;

            // Navigasyonu temizleme sonrası güncelle
            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgModuller);
            }
        }
    }
}