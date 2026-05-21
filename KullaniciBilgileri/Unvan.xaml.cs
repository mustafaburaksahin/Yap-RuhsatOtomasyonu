using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Login;
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu.KullaniciBilgileri
{
    public partial class UnvanPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        string aktifKullanici = OturumYonetimi.GirisYapanKullanici?.AdSoyad ?? "Bilinmeyen Kullanıcı";

        public UnvanPage()
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

            var yetki = _context.Yetkiler.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi && (x.Panel == "Ünvanlar" || x.Panel == "Ünvan"));
            bool islemYetkisiVar = yetki != null && (yetki.EkleYetki || yetki.GuncelleYetki);

            if (Panel1 != null) Panel1.Visibility = islemYetkisiVar ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TabloyuYenile()
        {
            try
            {
                var liste = _context.Unvanlar.ToList();

                for (int i = 0; i < liste.Count; i++)
                {
                    liste[i].SiraNo = i + 1;
                }

                dgUnvanlar.ItemsSource = liste;

                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgUnvanlar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void VeriyiTabloyaEkle()
        {
            if (string.IsNullOrWhiteSpace(txtUnvan.Text))
            {
                MessageBox.Show("Lütfen Ünvan alanını doldurun!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string girilenUnvan = txtUnvan.Text.Trim();

            if (_context.Unvanlar.Any(x => x.UnvanAdi == girilenUnvan))
            {
                MessageBox.Show("Bu ünvan zaten sistemde kayıtlı!", "Mükerrer Kayıt", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var yeniUnvan = new UnvanModel
            {
                UnvanAdi = girilenUnvan,
                DigerBilgiler = txtDigerBilgiler.Text,
                Aciklama = txtAciklama.Text,
                KaydedenKullanici = aktifKullanici,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
            };

            _context.Unvanlar.Add(yeniUnvan);
            _context.SaveChanges();

            TabloyuYenile();
            FormuTemizle();
            MessageBox.Show("Ünvan başarıyla eklendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgUnvanlar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUnvanlar.SelectedItem is UnvanModel secilen)
            {
                txtId.Text = secilen.SiraNo.ToString();
                txtUnvan.Text = secilen.UnvanAdi;
                txtDigerBilgiler.Text = secilen.DigerBilgiler;
                txtAciklama.Text = secilen.Aciklama;

                txtKaydeden.Text = secilen.KaydedenKullanici;
                txtKayitTarihi.Text = secilen.KayitTarihi;
                txtDegistiren.Text = secilen.DegistirenKullanici;

                // Seçilen kaydın değiştirilme tarihi UI'a aktarılıyor (hata vermemesi için null kontrolü eklendi)
                if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = secilen.DegistirmeTarihi;

                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgUnvanlar);
                }
            }
        }

        public void VeriyiSil()
        {
            if (dgUnvanlar.SelectedItem is UnvanModel secilen)
            {
                if (_context.Kullanicilar.Any(x => x.UnvanId == secilen.Id))
                {
                    MessageBox.Show("Bu ünvana bağlı kayıtlı sistem kullanıcıları mevcut! Önce o kullanıcıların ünvanını değiştirmeniz gerekir.", "Silme Engellendi", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                var sonuc = MessageBox.Show("Seçili ünvanı silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (sonuc == MessageBoxResult.Yes)
                {
                    var kayit = _context.Unvanlar.Find(secilen.Id);
                    if (kayit != null)
                    {
                        _context.Unvanlar.Remove(kayit);
                        _context.SaveChanges();

                        TabloyuYenile();
                        FormuTemizle();
                        MessageBox.Show("Ünvan başarıyla silindi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz ünvanı tablodan seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void VeriyiGuncelle()
        {
            if (dgUnvanlar.SelectedItem is UnvanModel secilen)
            {
                if (string.IsNullOrWhiteSpace(txtUnvan.Text))
                {
                    MessageBox.Show("Ünvan alanı boş bırakılamaz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var kayit = _context.Unvanlar.Find(secilen.Id);
                if (kayit != null)
                {
                    kayit.UnvanAdi = txtUnvan.Text.Trim();
                    kayit.DigerBilgiler = txtDigerBilgiler.Text;
                    kayit.Aciklama = txtAciklama.Text;

                    // Güncelleme yapan kullanıcı ve Tarih loglanıyor
                    kayit.DegistirenKullanici = aktifKullanici;
                    kayit.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    _context.SaveChanges();

                    TabloyuYenile();
                    FormuTemizle();
                    MessageBox.Show("Ünvan başarıyla güncellendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz ünvanı tablodan seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void FormuTemizle()
        {
            txtId.Clear();
            txtUnvan.Clear();
            txtDigerBilgiler.Clear();
            txtAciklama.Clear();

            txtKaydeden.Clear();
            txtKayitTarihi.Clear();
            txtDegistiren.Clear();

            // Log textbox'ı sıfırlanıyor
            if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Clear();

            dgUnvanlar.SelectedItem = null;

            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgUnvanlar);
            }
        }
    }
}