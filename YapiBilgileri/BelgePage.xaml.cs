using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Login;
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu.YapiBilgileri
{
    public partial class BelgePage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        // GÜNCELLEME: Aktif kullanıcı bilgisi sınıf seviyesine alındı
        string aktifKullaniciAdSoyad = OturumYonetimi.GirisYapanKullanici?.AdSoyad ?? "Bilinmeyen Kullanıcı";

        public BelgePage()
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
            if (user.KullaniciAdi == "admin")
            {
                if (Panel1 != null) Panel1.Visibility = Visibility.Visible;
                return;
            }

            // 2. VERİTABANI YETKİ KONTROLÜ
            var yetki = _context.Yetkiler.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi && x.Panel == "Belge Tipleri");

            // 3. YAPILAR SAYFASINDAKİ TEMİZ MANTIK
            bool islemYetkisiVar = yetki != null && (yetki.EkleYetki || yetki.GuncelleYetki);

            // 4. GÖRÜNÜRLÜK ATAMASI
            if (Panel1 != null) Panel1.Visibility = islemYetkisiVar ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TabloyuYenile()
        {
            try
            {
                var liste = _context.BelgeTipleri.ToList();

                for (int i = 0; i < liste.Count; i++)
                {
                    liste[i].SiraNo = i + 1;
                }

                dgBelgeTipleri.ItemsSource = liste;

                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgBelgeTipleri);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void VeriyiTabloyaEkle()
        {
            if (string.IsNullOrWhiteSpace(txtBelgeTipi.Text))
            {
                MessageBox.Show("Lütfen Belge Tipi alanını doldurun!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string girilenAd = txtBelgeTipi.Text.Trim();

            // Mükerrer kayıt kontrolü
            if (_context.BelgeTipleri.Any(x => x.Ad == girilenAd))
            {
                MessageBox.Show("Bu belge tipi zaten sistemde kayıtlı!", "Mükerrer Kayıt", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var yeniBelge = new BelgeTipiModel
            {
                Ad = girilenAd,
                DigerBilgiler = txtDigerBilgiler.Text,
                Aciklama = txtAciklama.Text,

                // GÜNCELLEME: Sınıf seviyesindeki değişken kullanılıyor
                KaydedenKullanici = aktifKullaniciAdSoyad,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
            };

            _context.BelgeTipleri.Add(yeniBelge);
            _context.SaveChanges();

            TabloyuYenile();
            FormuTemizle();
            MessageBox.Show("Kayıt başarıyla eklendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgBelgeTipleri_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBelgeTipleri.SelectedItem is BelgeTipiModel secilenKayit)
            {
                txtId.Text = secilenKayit.SiraNo.ToString();
                txtBelgeTipi.Text = secilenKayit.Ad;
                txtDigerBilgiler.Text = secilenKayit.DigerBilgiler;
                txtAciklama.Text = secilenKayit.Aciklama;

                // TextBox olarak güncellenen kilitli arayüz alanına veri yazdırılıyor
                if (txtKaydeden != null) txtKaydeden.Text = secilenKayit.KaydedenKullanici;
                if (txtKayitTarihi != null) txtKayitTarihi.Text = secilenKayit.KayitTarihi;

                // GÜNCELLEME: Değişiklik logları arayüze basılıyor
                if (txtDegistiren != null) txtDegistiren.Text = secilenKayit.DegistirenKullanici;
                if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = secilenKayit.DegistirmeTarihi;

                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgBelgeTipleri);
                }
            }
        }

        public void VeriyiSil()
        {
            if (dgBelgeTipleri.SelectedItem is BelgeTipiModel secilenKayit)
            {
                var sonuc = MessageBox.Show("Seçili belge tipini silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (sonuc == MessageBoxResult.Yes)
                {
                    try
                    {
                        var silinecekKayit = _context.BelgeTipleri.Find(secilenKayit.Id);
                        if (silinecekKayit != null)
                        {
                            _context.BelgeTipleri.Remove(silinecekKayit);
                            _context.SaveChanges();

                            TabloyuYenile();
                            FormuTemizle();
                            MessageBox.Show("Kayıt başarıyla silindi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Bu belge tipine bağlı kayıtlı yapı belgesi mevcut! Önce o belgeleri silmeniz veya değiştirmeniz gerekir.", "Silme Engellendi", MessageBoxButton.OK, MessageBoxImage.Stop);

                        _context.Entry(secilenKayit).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
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
            if (dgBelgeTipleri.SelectedItem is BelgeTipiModel secilenKayit)
            {
                if (string.IsNullOrWhiteSpace(txtBelgeTipi.Text))
                {
                    MessageBox.Show("Belge Tipi alanı boş bırakılamaz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var guncellenecekKayit = _context.BelgeTipleri.Find(secilenKayit.Id);

                if (guncellenecekKayit != null)
                {
                    guncellenecekKayit.Ad = txtBelgeTipi.Text.Trim();
                    guncellenecekKayit.DigerBilgiler = txtDigerBilgiler.Text;
                    guncellenecekKayit.Aciklama = txtAciklama.Text;

                    // GÜNCELLEME: Güncellemeyi yapan kullanıcı ve tarih loglanıyor
                    guncellenecekKayit.DegistirenKullanici = aktifKullaniciAdSoyad;
                    guncellenecekKayit.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    if (txtDegistiren != null) txtDegistiren.Text = guncellenecekKayit.DegistirenKullanici;
                    if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = guncellenecekKayit.DegistirmeTarihi;

                    _context.SaveChanges();

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
            txtBelgeTipi.Clear();
            txtDigerBilgiler.Clear();
            txtAciklama.Clear();

            // TextBox temizleme metotları entegre edildi
            if (txtKaydeden != null) txtKaydeden.Clear();
            if (txtKayitTarihi != null) txtKayitTarihi.Clear();

            // GÜNCELLEME: Log TextBox'ları sıfırlanıyor
            if (txtDegistiren != null) txtDegistiren.Clear();
            if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Clear();

            dgBelgeTipleri.SelectedItem = null;

            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgBelgeTipleri);
            }
        }
    }
}