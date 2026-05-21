using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Models;
using YapıRuhsatOtomasyonu.Login;

namespace YapıRuhsatOtomasyonu.YapiBilgileri
{
    public partial class IskanPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        // GÜNCELLEME: Aktif kullanıcı bilgisi sınıf seviyesine alındı
        string aktifKullaniciAdSoyad = OturumYonetimi.GirisYapanKullanici?.AdSoyad ?? "Bilinmeyen Kullanıcı";

        public IskanPage()
        {
            InitializeComponent();
            TabloyuYenile();

            this.Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = OturumYonetimi.GirisYapanKullanici;
            if (user == null || string.IsNullOrEmpty(user.KullaniciAdi)) return;

            // 1. SÜPER ADMİN KONTROLÜ: "admin" kullanıcısı yetki tablosuna takılmadan formu her zaman görür.
            if (user.KullaniciAdi == "admin")
            {
                if (Panel1 != null) Panel1.Visibility = Visibility.Visible;
                return;
            }

            // 2. DİĞER KULLANICILAR İÇİN VERİTABANI KONTROLÜ
            var yetki = _context.Yetkiler.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi && (x.Panel == "İskan Veriliş Amacı" || x.Panel == "İskan Amaçları"));

            // 3. Yapılar sayfasındaki temiz "boolean" mantığı: Yetki var mı VE Ekle veya Güncelle'den biri açık mı?
            bool islemYetkisiVar = yetki != null && (yetki.EkleYetki || yetki.GuncelleYetki);

            // 4. Tek satırda görünürlük ataması
            if (Panel1 != null) Panel1.Visibility = islemYetkisiVar ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TabloyuYenile()
        {
            try
            {
                var liste = _context.IskanAmaclari.ToList();

                for (int i = 0; i < liste.Count; i++)
                {
                    liste[i].SiraNo = i + 1;
                }

                dgIskanAmaclari.ItemsSource = liste;

                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgIskanAmaclari);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void VeriyiTabloyaEkle()
        {
            if (string.IsNullOrWhiteSpace(txtVerilisAmaci.Text))
            {
                MessageBox.Show("Lütfen Veriliş Amacı alanını doldurun!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string girilenAmac = txtVerilisAmaci.Text.Trim();

            // Mükerrer (Aynı isimde) kayıt kontrolü
            if (_context.IskanAmaclari.Any(x => x.VerilisAmaci == girilenAmac))
            {
                MessageBox.Show("Bu iskan veriliş amacı zaten sistemde kayıtlı!", "Mükerrer Kayıt", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var yeniKayit = new IskanAmaciModel
            {
                VerilisAmaci = girilenAmac,
                DigerBilgiler = txtDigerBilgiler.Text,
                Aciklama = txtAciklama.Text,

                // Sistem otomatik mühürleme yapıyor
                KaydedenKullanici = aktifKullaniciAdSoyad,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
            };

            _context.IskanAmaclari.Add(yeniKayit);
            _context.SaveChanges();

            TabloyuYenile();
            FormuTemizle();
            MessageBox.Show("Kayıt başarıyla eklendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgIskanAmaclari_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgIskanAmaclari.SelectedItem is IskanAmaciModel secilenKayit)
            {
                txtId.Text = secilenKayit.SiraNo.ToString();
                txtVerilisAmaci.Text = secilenKayit.VerilisAmaci;
                txtDigerBilgiler.Text = secilenKayit.DigerBilgiler;
                txtAciklama.Text = secilenKayit.Aciklama;

                // TextBox olarak güncellenen kilitli arayüz alanına veri yazdırılıyor
                if (txtKaydeden != null) txtKaydeden.Text = secilenKayit.KaydedenKullanici;
                if (txtKayitTarihi != null) txtKayitTarihi.Text = secilenKayit.KayitTarihi;

                // GÜNCELLEME: Değişiklik yapan kullanıcı ve tarih bilgileri arayüze basılıyor
                if (txtDegistiren != null) txtDegistiren.Text = secilenKayit.DegistirenKullanici;
                if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = secilenKayit.DegistirmeTarihi;
            }
        }

        public void VeriyiSil()
        {
            if (dgIskanAmaclari.SelectedItem is IskanAmaciModel secilenKayit)
            {
                // İlişkisel bütünlük kısıtlaması (Foreign Key Koruması)
                if (_context.BelgeDetay.Any(x => x.IskanVerilisAmaciId == secilenKayit.Id))
                {
                    MessageBox.Show("Bu iskan amacına bağlı kayıtlı yapı ruhsatı/belgesi mevcut! Önce o belgeleri silmeniz veya değiştirmeniz gerekir.", "Silme Engellendi", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                var sonuc = MessageBox.Show("Seçili kaydı silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (sonuc == MessageBoxResult.Yes)
                {
                    var silinecekKayit = _context.IskanAmaclari.Find(secilenKayit.Id);
                    if (silinecekKayit != null)
                    {
                        _context.IskanAmaclari.Remove(silinecekKayit);
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
            if (dgIskanAmaclari.SelectedItem is IskanAmaciModel secilenKayit)
            {
                if (string.IsNullOrWhiteSpace(txtVerilisAmaci.Text))
                {
                    MessageBox.Show("Veriliş Amacı alanı boş bırakılamaz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var guncellenecekKayit = _context.IskanAmaclari.Find(secilenKayit.Id);
                if (guncellenecekKayit != null)
                {
                    guncellenecekKayit.VerilisAmaci = txtVerilisAmaci.Text.Trim();
                    guncellenecekKayit.DigerBilgiler = txtDigerBilgiler.Text;
                    guncellenecekKayit.Aciklama = txtAciklama.Text;

                    // GÜNCELLEME: Güncellemeyi yapan kullanıcı ve tarih bilgisi modele işleniyor
                    guncellenecekKayit.DegistirenKullanici = aktifKullaniciAdSoyad;
                    guncellenecekKayit.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    // Form alanları anlık güncelleniyor
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
            txtId.Clear();
            txtVerilisAmaci.Clear();
            txtDigerBilgiler.Clear();
            txtAciklama.Clear();

            // TextBox temizleme metotları
            if (txtKaydeden != null) txtKaydeden.Clear();
            if (txtKayitTarihi != null) txtKayitTarihi.Clear();

            // GÜNCELLEME: Değiştiren ve değiştirilme tarihi alanları temizleniyor
            if (txtDegistiren != null) txtDegistiren.Clear();
            if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Clear();

            dgIskanAmaclari.SelectedItem = null;

            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgIskanAmaclari);
            }
        }
    }
}