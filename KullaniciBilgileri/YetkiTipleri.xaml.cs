using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Models;
using YapıRuhsatOtomasyonu.Login; // Main sınıfına erişim için

namespace YapıRuhsatOtomasyonu.KullaniciBilgileri
{
    public partial class YetkiTipleriPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        public YetkiTipleriPage()
        {
            InitializeComponent();
            TabloyuYenile();
        }

        private void TabloyuYenile()
        {
            // 1. Tabloyu veritabanından çek
            var liste = _context.YetkiTipleri.ToList();

            // Sanal Sıra Numaralarını ata (Görsel kaymayı engeller)
            for (int i = 0; i < liste.Count; i++)
            {
                liste[i].SiraNo = i + 1;
            }

            dgYetkiTipleri.ItemsSource = liste;

            // 2. Kullanıcı listesini çek ve ComboBox'lara yükle
            var kullanicilar = _context.Kullanicilar.Select(k => k.AdSoyad).ToList();
            cmbKaydeden.ItemsSource = kullanicilar;
            cmbDegistiren.ItemsSource = kullanicilar;

            // 3. Ana penceredeki navigasyon göstergesini güncelle (1 / X kısmı)
            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgYetkiTipleri);
            }
        }

        public void VeriyiTabloyaEkle()
        {
            if (string.IsNullOrWhiteSpace(txtYetkiTipi.Text))
            {
                MessageBox.Show("Lütfen Yetki Tipi alanını doldurun!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var yeniTip = new YetkiTipiModel
            {
                YetkiTipiAdi = txtYetkiTipi.Text,
                DigerBilgiler = txtDigerBilgiler.Text,
                Aciklama = txtAciklama.Text,

                // Log Alanları
                KaydedenKullanici = cmbKaydeden.Text,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                DegistirenKullanici = "",
                DegistirmeTarihi = ""
            };

            _context.YetkiTipleri.Add(yeniTip);
            _context.SaveChanges();

            TabloyuYenile();
            FormuTemizle();
            MessageBox.Show("Yetki tipi başarıyla eklendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgYetkiTipleri_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgYetkiTipleri.SelectedItem is YetkiTipiModel secilen)
            {
                // Form alanlarını doldur - ID kutusuna SiraNo basılıyor
                txtId.Text = secilen.SiraNo.ToString();

                txtYetkiTipi.Text = secilen.YetkiTipiAdi;
                txtDigerBilgiler.Text = secilen.DigerBilgiler;
                txtAciklama.Text = secilen.Aciklama;
                cmbKaydeden.Text = secilen.KaydedenKullanici;
                txtKayitTarihi.Text = secilen.KayitTarihi;
                cmbDegistiren.Text = secilen.DegistirenKullanici;
                txtDegistirmeTarihi.Text = secilen.DegistirmeTarihi;

                // Navigasyon Göstergesini Güncelle
                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgYetkiTipleri);
                }
            }
        }

        public void VeriyiSil()
        {
            if (dgYetkiTipleri.SelectedItem is YetkiTipiModel secilen)
            {
                var sonuc = MessageBox.Show("Seçili yetki tipini silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (sonuc == MessageBoxResult.Yes)
                {
                    // Silme işlemi GERÇEK ID üzerinden yapılır
                    var kayit = _context.YetkiTipleri.Find(secilen.Id);
                    if (kayit != null)
                    {
                        _context.YetkiTipleri.Remove(kayit);
                        _context.SaveChanges();

                        TabloyuYenile();
                        FormuTemizle();
                        MessageBox.Show("Kayıt başarıyla silindi.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz kaydı tablodan seçin!");
            }
        }

        public void VeriyiGuncelle()
        {
            if (dgYetkiTipleri.SelectedItem is YetkiTipiModel secilen)
            {
                // Güncelleme işlemi GERÇEK ID üzerinden yapılır
                var kayit = _context.YetkiTipleri.Find(secilen.Id);
                if (kayit != null)
                {
                    kayit.YetkiTipiAdi = txtYetkiTipi.Text;
                    kayit.DigerBilgiler = txtDigerBilgiler.Text;
                    kayit.Aciklama = txtAciklama.Text;

                    // Log alanları güncelleme
                    kayit.DegistirenKullanici = cmbDegistiren.Text;
                    kayit.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    _context.SaveChanges();

                    TabloyuYenile();
                    FormuTemizle();
                    MessageBox.Show("Yetki tipi başarıyla güncellendi.");
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz kaydı tablodan seçin.");
            }
        }

        public void FormuTemizle()
        {
            txtId.Clear();
            txtYetkiTipi.Clear();
            txtDigerBilgiler.Clear();
            txtAciklama.Clear();

            cmbKaydeden.SelectedIndex = -1;
            txtKayitTarihi.Clear();
            cmbDegistiren.SelectedIndex = -1;
            txtDegistirmeTarihi.Clear();

            dgYetkiTipleri.SelectedItem = null;

            // Temizleme sonrası navigasyonu sıfırla/güncelle
            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgYetkiTipleri);
            }
        }
    }
}