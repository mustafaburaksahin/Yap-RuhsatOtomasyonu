using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Models;
using YapıRuhsatOtomasyonu.Login;

namespace YapıRuhsatOtomasyonu.SicilBilgileri
{
    public partial class SicilTipleriPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        // GÜNCELLEME: Aktif kullanıcı bilgisi sınıf seviyesinde
        string aktifKullanici = OturumYonetimi.GirisYapanKullanici?.AdSoyad ?? "Bilinmeyen Kullanıcı";

        public SicilTipleriPage()
        {
            InitializeComponent();
            TabloyuYenile();

            this.Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = OturumYonetimi.GirisYapanKullanici;
            if (user == null || string.IsNullOrEmpty(user.KullaniciAdi)) return;

            // 1. SÜPER ADMİN KONTROLÜ: Sadece "admin" kullanıcısı formu her zaman görür.
            if (user.KullaniciAdi == "admin")
            {
                if (Panel1 != null) Panel1.Visibility = Visibility.Visible;
                return;
            }

            // 2. VERİTABANI KONTROLÜ: using bloğu kaldırıldı, global _context kullanıldı.
            var yetki = _context.Yetkiler.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi && (x.Panel == "Sicil Tipleri" || x.Panel == "Sicil Tipi"));

            // 3. TEMİZ BOOLEAN MANTIĞI: Yetki var mı VE Ekle veya Güncelle'den biri açık mı?
            bool islemYetkisiVar = yetki != null && (yetki.EkleYetki || yetki.GuncelleYetki);

            // 4. GÖRÜNÜRLÜK ATAMASI (Güvenlik açığı kapatıldı, yetkisiz ise Collapsed olur)
            if (Panel1 != null) Panel1.Visibility = islemYetkisiVar ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TabloyuYenile()
        {
            try
            {
                var liste = _context.SicilTipleri.ToList();

                for (int i = 0; i < liste.Count; i++)
                {
                    liste[i].SiraNo = i + 1;
                }

                dgSicilTipleri.ItemsSource = liste;

                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgSicilTipleri);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void VeriyiTabloyaEkle()
        {
            if (string.IsNullOrWhiteSpace(txtSicilTipi.Text))
            {
                MessageBox.Show("Lütfen Sicil Tipi alanını doldurun!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string girilenTip = txtSicilTipi.Text.Trim();

            // GÜNCELLEME: Mükerrer (Aynı isimde) kayıt kontrolü eklendi
            if (_context.SicilTipleri.Any(x => x.SicilTipi == girilenTip))
            {
                MessageBox.Show("Bu sicil tipi zaten sistemde kayıtlı!", "Mükerrer Kayıt", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var yeniSicilTipi = new SicilTipiModel
            {
                SicilTipi = girilenTip,
                DigerBilgiler = txtDigerBilgiler.Text,
                Aciklama = txtAciklama.Text,

                // GÜNCELLEME: Giriş yapan kullanıcının gerçek adı el değmeden mühürleniyor
                KaydedenKullanici = aktifKullanici,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
            };

            _context.SicilTipleri.Add(yeniSicilTipi);
            _context.SaveChanges();

            TabloyuYenile();
            FormuTemizle();
            MessageBox.Show("Kayıt başarıyla eklendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgSicilTipleri_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSicilTipleri.SelectedItem is SicilTipiModel secilenKayit)
            {
                txtId.Text = secilenKayit.SiraNo.ToString();
                txtSicilTipi.Text = secilenKayit.SicilTipi;
                txtDigerBilgiler.Text = secilenKayit.DigerBilgiler;
                txtAciklama.Text = secilenKayit.Aciklama;

                // GÜNCELLEME: Log bilgileri arayüze yazdırılıyor
                if (txtKaydeden != null) txtKaydeden.Text = secilenKayit.KaydedenKullanici;
                if (txtKayitTarihi != null) txtKayitTarihi.Text = secilenKayit.KayitTarihi;
                if (txtDegistiren != null) txtDegistiren.Text = secilenKayit.DegistirenKullanici;
                if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = secilenKayit.DegistirmeTarihi; // EKLENDİ

                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgSicilTipleri);
                }
            }
        }

        public void VeriyiSil()
        {
            if (dgSicilTipleri.SelectedItem is SicilTipiModel secilenKayit)
            {
                // GÜNCELLEME: İlişkisel bütünlük kontrolü. 
                if (_context.Siciller.Any(x => x.SicilTipi == secilenKayit.Id))
                {
                    MessageBox.Show("Bu sicil tipine bağlı personel/firma kayıtları mevcut! Önce o kayıtları silmeniz veya değiştirmeniz gerekir.", "Silme Engellendi", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                var sonuc = MessageBox.Show("Seçili kaydı silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (sonuc == MessageBoxResult.Yes)
                {
                    var silinecekKayit = _context.SicilTipleri.Find(secilenKayit.Id);

                    if (silinecekKayit != null)
                    {
                        _context.SicilTipleri.Remove(silinecekKayit);
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
            if (dgSicilTipleri.SelectedItem is SicilTipiModel secilenKayit)
            {
                if (string.IsNullOrWhiteSpace(txtSicilTipi.Text))
                {
                    MessageBox.Show("Sicil Tipi alanı boş bırakılamaz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var guncellenecekKayit = _context.SicilTipleri.Find(secilenKayit.Id);

                if (guncellenecekKayit != null)
                {
                    guncellenecekKayit.SicilTipi = txtSicilTipi.Text;
                    guncellenecekKayit.DigerBilgiler = txtDigerBilgiler.Text;
                    guncellenecekKayit.Aciklama = txtAciklama.Text;

                    // GÜNCELLEME: Güncelleme yapan kişi ve tarih loglanıyor
                    guncellenecekKayit.DegistirenKullanici = aktifKullanici;
                    guncellenecekKayit.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"); // EKLENDİ

                    if (txtDegistiren != null) txtDegistiren.Text = guncellenecekKayit.DegistirenKullanici;
                    if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = guncellenecekKayit.DegistirmeTarihi; // EKLENDİ

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
            txtSicilTipi.Clear();
            txtDigerBilgiler.Clear();
            txtAciklama.Clear();

            // GÜNCELLEME: TextBox sıfırlama metotları entegre edildi
            if (txtKaydeden != null) txtKaydeden.Clear();
            if (txtKayitTarihi != null) txtKayitTarihi.Clear();
            if (txtDegistiren != null) txtDegistiren.Clear();
            if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Clear(); // EKLENDİ

            dgSicilTipleri.SelectedItem = null;

            if (Window.GetWindow(this) is Main anaPencere)
            {
                anaPencere.UpdateNavDisplay(dgSicilTipleri);
            }
        }
    }
}