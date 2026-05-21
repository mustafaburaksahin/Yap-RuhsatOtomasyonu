using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YapıRuhsatOtomasyonu.Data; // Veritabanı bağlantımız için gerekli
using YapıRuhsatOtomasyonu.Login; // Main sayfasına erişim için eklendi
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu.KullaniciBilgileri
{
    public partial class KullaniciTipleri : Page
    {
        // Veritabanı bağlantı nesnemiz
        private readonly AppDbContext _context = new AppDbContext();

        public KullaniciTipleri()
        {
            InitializeComponent();
            TabloyuYenile(); // Sayfa açıldığında veritabanından verileri çek
        }

        // --- Verileri Veritabanından Çekip Tabloya Doldurma ---
        private void TabloyuYenile()
        {
            var liste = _context.KullaniciTipleri.ToList();

            // Sanal Sıra Numaralarını ata (Görsel kaymayı engeller)
            for (int i = 0; i < liste.Count; i++)
            {
                liste[i].SiraNo = i + 1;
            }

            dgKullaniciTipleri.ItemsSource = liste;

            // Log ComboBox'larını kullanıcı listesiyle doldur
            var kullaniciListesi = _context.Kullanicilar
                                           .Select(k => k.AdSoyad)
                                           .Where(k => k != null)
                                           .ToList();

            cmbKaydedenKullanici.ItemsSource = kullaniciListesi;
            cmbDegistirenKullanici.ItemsSource = kullaniciListesi;

            // --- NAVİGASYON GÜNCELLEME ---
            // Sayfa ilk açıldığında veya veri değiştiğinde Main'deki 1 / X kısmını günceller
            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgKullaniciTipleri);
            }
        }

        public void VeriyiTabloyaEkle()
        {
            if (string.IsNullOrWhiteSpace(txtKullaniciTipi.Text))
            {
                MessageBox.Show("Lütfen Kullanıcı Tipi alanını doldurun!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Yeni kayıt oluştur (ID atamıyoruz, veritabanı otomatik veriyor)
            var yeniKullaniciTipi = new KullaniciTipiModel
            {
                KullaniciTipi = txtKullaniciTipi.Text,
                DigerBilgiler = txtDigerBilgiler.Text,
                Aciklama = txtAciklama.Text,

                // Log Alanları
                KaydedenKullanici = cmbKaydedenKullanici.Text,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                DegistirenKullanici = "",
                DegistirmeTarihi = ""
            };

            // Veritabanına ekle ve kaydet
            _context.KullaniciTipleri.Add(yeniKullaniciTipi);
            _context.SaveChanges();

            TabloyuYenile(); // Tabloyu güncelle
            FormuTemizle();
            MessageBox.Show("Kayıt başarıyla eklendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgKullaniciTipleri_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgKullaniciTipleri.SelectedItem is KullaniciTipiModel secilenKayit)
            {
                // Görsel Sıra No Basılıyor
                txtId.Text = secilenKayit.SiraNo.ToString();

                txtKullaniciTipi.Text = secilenKayit.KullaniciTipi;
                txtDigerBilgiler.Text = secilenKayit.DigerBilgiler;
                txtAciklama.Text = secilenKayit.Aciklama;

                // Log Alanlarını Doldur
                cmbKaydedenKullanici.Text = secilenKayit.KaydedenKullanici;
                txtKayitTarihi.Text = secilenKayit.KayitTarihi;
                cmbDegistirenKullanici.Text = secilenKayit.DegistirenKullanici;
                txtDegistirmeTarihi.Text = secilenKayit.DegistirmeTarihi;

                // --- NAVİGASYON GÜNCELLEME ---
                // Tablodan seçim yapıldığında Main sayfasındaki sayacı (Örn: 3 / 10) günceller
                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgKullaniciTipleri);
                }
            }
        }

        public void VeriyiSil()
        {
            if (dgKullaniciTipleri.SelectedItem is KullaniciTipiModel secilenKayit)
            {
                // Silme onayı soralım
                var sonuc = MessageBox.Show("Seçili kullanıcı tipini silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (sonuc == MessageBoxResult.Yes)
                {
                    // Silinecek kaydı GERÇEK ID üzerinden veritabanından buluyoruz
                    var silinecekKayit = _context.KullaniciTipleri.Find(secilenKayit.Id);

                    if (silinecekKayit != null)
                    {
                        _context.KullaniciTipleri.Remove(silinecekKayit);
                        _context.SaveChanges(); // Değişikliği veritabanına yansıt

                        TabloyuYenile();
                        FormuTemizle();
                        MessageBox.Show("Kayıt başarıyla silindi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için tablodan bir kayıt seçin!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void VeriyiGuncelle()
        {
            if (dgKullaniciTipleri.SelectedItem is KullaniciTipiModel secilenKayit)
            {
                // Güncellenecek kaydı GERÇEK ID üzerinden veritabanından getir
                var guncellenecekKayit = _context.KullaniciTipleri.Find(secilenKayit.Id);

                if (guncellenecekKayit != null)
                {
                    guncellenecekKayit.KullaniciTipi = txtKullaniciTipi.Text;
                    guncellenecekKayit.DigerBilgiler = txtDigerBilgiler.Text;
                    guncellenecekKayit.Aciklama = txtAciklama.Text;

                    // Log güncellemesi
                    guncellenecekKayit.DegistirenKullanici = cmbDegistirenKullanici.Text;
                    guncellenecekKayit.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    _context.SaveChanges(); // SQL'e güncellemeyi kaydet

                    TabloyuYenile();
                    FormuTemizle();
                    MessageBox.Show("Kayıt başarıyla güncellendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek için tablodan bir kayıt seçin!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void FormuTemizle()
        {
            txtId.Text = string.Empty;
            txtKullaniciTipi.Clear();
            txtDigerBilgiler.Clear();
            txtAciklama.Clear();

            // Log alanlarını da temizle
            cmbKaydedenKullanici.SelectedIndex = -1;
            txtKayitTarihi.Clear();
            cmbDegistirenKullanici.SelectedIndex = -1;
            txtDegistirmeTarihi.Clear();

            dgKullaniciTipleri.SelectedItem = null;

            // Form temizlendiğinde sayacı (0 / X) durumuna getirmek için tetikle
            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgKullaniciTipleri);
            }
        }
    }
}