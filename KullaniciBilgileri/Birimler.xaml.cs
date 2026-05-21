using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Login; // Main ve OturumYonetimi erişimi için
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu.KullaniciBilgileri
{
    public partial class BirimPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        public BirimPage()
        {
            InitializeComponent();
            TabloyuYenile();

            this.Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = OturumYonetimi.GirisYapanKullanici;
            if (user == null || string.IsNullOrEmpty(user.KullaniciAdi)) return;

            // 1. SÜPER ADMİN KONTROLÜ
            if (user.KullaniciAdi == "admin" || user.KullaniciTipi == "Yönetici")
            {
                if (Panel1 != null) Panel1.Visibility = Visibility.Visible;
                return;
            }

            // 2. VERİTABANI KONTROLÜ
            var yetki = _context.Yetkiler.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi && (x.Panel == "Birimler" || x.Panel == "Birim"));

            // 3. TEMİZ BOOLEAN MANTIĞI
            bool islemYetkisiVar = yetki != null && (yetki.EkleYetki || yetki.GuncelleYetki);

            // 4. GÖRÜNÜRLÜK ATAMASI
            if (Panel1 != null) Panel1.Visibility = islemYetkisiVar ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TabloyuYenile()
        {
            try
            {
                var liste = _context.Birimler.ToList();

                for (int i = 0; i < liste.Count; i++)
                {
                    liste[i].SiraNo = i + 1;
                }

                // Önlem: Seçimi boşaltıp öyle dolduralım ki formlar anlık dolmasın
                dgBirimler.SelectedIndex = -1;
                dgBirimler.ItemsSource = null;
                dgBirimler.ItemsSource = liste;

                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgBirimler);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void VeriyiTabloyaEkle()
        {
            if (string.IsNullOrWhiteSpace(txtBirim.Text))
            {
                MessageBox.Show("Lütfen Birim alanını doldurun!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string girilenBirim = txtBirim.Text.Trim();

            if (_context.Birimler.Any(x => x.BirimAdi == girilenBirim))
            {
                MessageBox.Show("Bu birim zaten sistemde kayıtlı!", "Mükerrer Kayıt", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string aktifKullanici = OturumYonetimi.GirisYapanKullanici?.AdSoyad ?? "Bilinmeyen Kullanıcı";

            var yeniKayit = new BirimModel
            {
                BirimAdi = girilenBirim,
                DigerBilgiler = txtDigerBilgiler.Text,
                Aciklama = txtAciklama.Text,
                KaydedenKullanici = aktifKullanici,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
            };

            _context.Birimler.Add(yeniKayit);
            _context.SaveChanges();

            TabloyuYenile();
            FormuTemizle();
            MessageBox.Show("Kayıt başarıyla eklendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgBirimler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Null kontrolü eklendi: Tablo yenilendiğinde hata fırlatmaması için
            if (dgBirimler.SelectedItem == null) return;

            if (dgBirimler.SelectedItem is BirimModel secilenKayit)
            {
                txtId.Text = secilenKayit.SiraNo.ToString();
                txtBirim.Text = secilenKayit.BirimAdi;
                txtDigerBilgiler.Text = secilenKayit.DigerBilgiler;
                txtAciklama.Text = secilenKayit.Aciklama;

                txtKaydeden.Text = secilenKayit.KaydedenKullanici;
                txtKayitTarihi.Text = secilenKayit.KayitTarihi;

                if (txtDegistiren != null) txtDegistiren.Text = secilenKayit.DegistirenKullanici;
                if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = secilenKayit.DegistirmeTarihi;

                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgBirimler);
                }
            }
        }

        public void VeriyiSil()
        {
            if (dgBirimler.SelectedItem is BirimModel secilenKayit)
            {
                if (_context.Kullanicilar.Any(x => x.BirimId == secilenKayit.Id))
                {
                    MessageBox.Show("Bu birime bağlı kayıtlı sistem kullanıcıları mevcut! Önce o kullanıcıların birimini değiştirmeniz gerekir.", "Silme Engellendi", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                var sonuc = MessageBox.Show("Seçili kaydı silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (sonuc == MessageBoxResult.Yes)
                {
                    var silinecekKayit = _context.Birimler.Find(secilenKayit.Id);

                    if (silinecekKayit != null)
                    {
                        _context.Birimler.Remove(silinecekKayit);
                        _context.SaveChanges();

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
            if (dgBirimler.SelectedItem is BirimModel secilenKayit)
            {
                if (string.IsNullOrWhiteSpace(txtBirim.Text))
                {
                    MessageBox.Show("Birim alanı boş bırakılamaz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var guncellenecekKayit = _context.Birimler.Find(secilenKayit.Id);

                if (guncellenecekKayit != null)
                {
                    string aktifKullanici = OturumYonetimi.GirisYapanKullanici?.AdSoyad ?? "Bilinmeyen Kullanıcı";

                    guncellenecekKayit.BirimAdi = txtBirim.Text.Trim();
                    guncellenecekKayit.DigerBilgiler = txtDigerBilgiler.Text;
                    guncellenecekKayit.Aciklama = txtAciklama.Text;

                    guncellenecekKayit.DegistirenKullanici = aktifKullanici;
                    guncellenecekKayit.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    _context.SaveChanges();

                    TabloyuYenile();
                    MessageBox.Show("Kayıt başarıyla güncellendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    FormuTemizle();
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
            txtBirim.Clear();
            txtDigerBilgiler.Clear();
            txtAciklama.Clear();

            txtKaydeden.Clear();
            txtKayitTarihi.Clear();

            if (txtDegistiren != null) txtDegistiren.Clear();
            if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Clear();

            // Seçimi iptal et ki UI üzerindeki SelectionChanged boşa tetiklenmesin
            dgBirimler.SelectedIndex = -1;
            dgBirimler.SelectedItem = null;

            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgBirimler);
            }
        }
    }
}