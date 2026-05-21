using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection; // Reflection eklendi (Dinamik özellik okuma için)
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Login;
using YapıRuhsatOtomasyonu.Models;

namespace YapıRuhsatOtomasyonu.KullaniciBilgileri
{
    public partial class KullanicilarPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        string aktifKullanici = OturumYonetimi.GirisYapanKullanici?.AdSoyad ?? "Bilinmeyen Kullanıcı";
        private bool IsSuperAdmin => OturumYonetimi.GirisYapanKullanici?.KullaniciAdi == "admin";

        // Filtreleme için tanımlar
        private ICollectionView _kullanicilarView = null!;
        private Dictionary<string, string> _filtreler = new Dictionary<string, string>();

        public KullanicilarPage()
        {
            InitializeComponent();

            if (OturumYonetimi.GirisYapanKullanici?.KullaniciTipi != "Admin")
            {
                MessageBox.Show("Bu sayfaya erişim yetkiniz bulunmamaktadır!", "Yetki Engeli", MessageBoxButton.OK, MessageBoxImage.Stop);
                this.IsEnabled = false;
                return;
            }

            cmbKullaniciTipi.Items.Clear();
            if (IsSuperAdmin) { cmbKullaniciTipi.Items.Add("Admin"); cmbKullaniciTipi.Items.Add("User"); }
            else { cmbKullaniciTipi.Items.Add("User"); }

            txtAnneAdi.PreviewTextInput += SadeceHarf_PreviewTextInput;
            txtBabaAdi.PreviewTextInput += SadeceHarf_PreviewTextInput;
            txtAdSoyad.PreviewTextInput += SadeceHarf_PreviewTextInput;

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

            var yetki = _context.Yetkiler.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi && (x.Panel == "Siciller" || x.Panel == "Sicil"));
            bool islemYetkisiVar = yetki != null && (yetki.EkleYetki || yetki.GuncelleYetki);
            if (Panel1 != null) Panel1.Visibility = islemYetkisiVar ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SadeceHarf_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        private void TabloyuYenile()
        {
            try
            {
                var girisYapan = OturumYonetimi.GirisYapanKullanici;
                if (girisYapan == null) return;

                var sorgu = _context.Kullanicilar.Include(x => x.Unvan).Include(x => x.Birim).Where(x => x.KullaniciAdi != "admin");
                if (!IsSuperAdmin) sorgu = sorgu.Where(x => x.KullaniciTipi == "User" && x.Id != girisYapan.Id);

                var liste = sorgu.ToList();
                for (int i = 0; i < liste.Count; i++) { liste[i].SiraNo = i + 1; }

                // Filtreleme mimarisi bağlaması
                _kullanicilarView = CollectionViewSource.GetDefaultView(liste);
                _kullanicilarView.Filter = KullaniciFiltreMantigi;
                dgKullanicilar.ItemsSource = _kullanicilarView;

                cmbUnvan.ItemsSource = _context.Unvanlar.ToList();
                cmbUnvan.DisplayMemberPath = "UnvanAdi"; cmbUnvan.SelectedValuePath = "Id";
                cmbBirim.ItemsSource = _context.Birimler.ToList();
                cmbBirim.DisplayMemberPath = "BirimAdi"; cmbBirim.SelectedValuePath = "Id";

                FormuTemizle();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        // METİN (TextBox) Sütunları İçin Filtre Tetikleyici
        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb && tb.Tag != null)
            {
                _filtreler[tb.Tag.ToString()!] = tb.Text.Trim();
                _kullanicilarView?.Refresh();
            }
        }

        // AKTİF/PASİF (ComboBox) Sütunları İçin Filtre Tetikleyici
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cb && cb.Tag != null && cb.SelectedItem is ComboBoxItem item)
            {
                string column = cb.Tag.ToString()!;
                string deger = item.Content.ToString()!;

                // "Tümü" seçildiyse filtreyi temizle, değilse değeri ata
                _filtreler[column] = deger == "Tümü" ? string.Empty : deger;
                _kullanicilarView?.Refresh();
            }
        }

        // DİNAMİK FİLTRELEME MANTIĞI
        private bool KullaniciFiltreMantigi(object item)
        {
            if (!(item is KullaniciModel k)) return false;

            foreach (var filter in _filtreler)
            {
                // Kutucuk boşsa o sütunu filtreleme
                if (string.IsNullOrWhiteSpace(filter.Value)) continue;

                string columnName = filter.Key;
                string searchValue = filter.Value.ToLower(); // Kolay eşleşme için küçük harfe çevir

                // Tag ile belirtilen özelliği Reflection ile bul
                object? propertyValue = GetPropertyValue(k, columnName);

                // Eğer özellik o satırda null ise ve biz bir şey arıyorsak, eşleşmedi demektir
                if (propertyValue == null) return false;

                // Checkbox (AktifMi) özel kontrolü
                if (columnName == "AktifMi")
                {
                    bool isAktif = (bool)propertyValue;
                    if (filter.Value == "Aktif" && !isAktif) return false;
                    if (filter.Value == "Pasif" && isAktif) return false;
                    continue;
                }

                // Standart string kontrolü
                if (!propertyValue.ToString()!.ToLower().Contains(searchValue))
                {
                    return false; // İçinde geçmiyorsa bu satırı gizle
                }
            }

            return true; // Tüm filtre şartlarını sağlıyorsa göster
        }

        // Unvan.UnvanAdi gibi iç içe propları okuyan yardımcı metot
        private object? GetPropertyValue(object obj, string propertyName)
        {
            foreach (var prop in propertyName.Split('.'))
            {
                if (obj == null) return null;
                PropertyInfo? pi = obj.GetType().GetProperty(prop);
                if (pi == null) return null;
                obj = pi.GetValue(obj, null)!;
            }
            return obj;
        }

        public void VeriyiTabloyaEkle()
        {
            // 1. Formdaki TÜM verileri geçici bir modele alıyoruz
            var yeniKullanici = new KullaniciModel
            {
                AktifMi = chkAktifMi.IsChecked ?? false,
                TcKimlikNo = txtTcKimlikNo.Text,
                AdSoyad = txtAdSoyad.Text,
                DogumYeri = txtDogumYeri.Text,
                DogumTarihi = dpDogumTarihi.Text,
                AnneAdi = txtAnneAdi.Text,
                BabaAdi = txtBabaAdi.Text,
                TelefonNo = txtTelefonNo.Text,
                GsmNo = txtGsmNo.Text,
                Email = txtEmail.Text,
                KullaniciAdi = txtKullaniciAdi.Text,
                Sifre = txtSifre.Text,
                KullaniciTipi = cmbKullaniciTipi.Text,
                UnvanId = (int?)cmbUnvan.SelectedValue,
                BirimId = (int?)cmbBirim.SelectedValue,
                Tanim = txtTanim.Text,
                Adres = txtAdres.Text,
                DigerBilgiler = txtDigerBilgiler.Text,
                Aciklama = txtAciklama.Text,

                // Log kayıtlarını (metadata) ekle
                KaydedenKullanici = aktifKullanici,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
            };

            // 2. Doğrulama başarısız olursa işlemi iptal et
            if (!KullaniciDogrula(yeniKullanici, "Ekle")) return;

            // 3. Doğrulama BAŞARILIYSA, modeli veritabanına ekle
            _context.Kullanicilar.Add(yeniKullanici);
            _context.SaveChanges();
            TabloyuYenile();
            FormuTemizle(); // Ekleme sonrası formu temizlemek iyi bir kullanıcı deneyimidir
            MessageBox.Show("Kayıt başarıyla tamamlandı.");
        }

        public void VeriyiGuncelle()
        {
            if (dgKullanicilar.SelectedItem is KullaniciModel s)
            {
                var g = _context.Kullanicilar.Find(s.Id);
                if (g != null)
                {
                    // 1. Formdaki TÜM verileri geçici bir modele alıyoruz
                    var dogrulanacakKullanici = new KullaniciModel
                    {
                        Id = g.Id, // Kendi Id'sini veriyoruz (Benzersizlik kontrolü kendisiyle çakışmasın diye)
                        AktifMi = chkAktifMi.IsChecked ?? false,
                        TcKimlikNo = txtTcKimlikNo.Text,
                        AdSoyad = txtAdSoyad.Text,
                        DogumYeri = txtDogumYeri.Text,
                        DogumTarihi = dpDogumTarihi.Text,
                        AnneAdi = txtAnneAdi.Text,
                        BabaAdi = txtBabaAdi.Text,
                        TelefonNo = txtTelefonNo.Text,
                        GsmNo = txtGsmNo.Text,
                        Email = txtEmail.Text,
                        KullaniciAdi = txtKullaniciAdi.Text,
                        // Şifre alanı boş bırakılmışsa eski şifreyi koruyoruz
                        Sifre = string.IsNullOrEmpty(txtSifre.Text) ? g.Sifre : txtSifre.Text,
                        KullaniciTipi = cmbKullaniciTipi.Text,
                        UnvanId = (int?)cmbUnvan.SelectedValue,
                        BirimId = (int?)cmbBirim.SelectedValue,
                        Tanim = txtTanim.Text,
                        Adres = txtAdres.Text,
                        DigerBilgiler = txtDigerBilgiler.Text,
                        Aciklama = txtAciklama.Text
                    };

                    // 2. Doğrulama başarısız olursa işlemi iptal et
                    if (!KullaniciDogrula(dogrulanacakKullanici, "Güncelle")) return;

                    // 3. Doğrulama BAŞARILIYSA, geçici modeldeki temiz verileri veritabanı nesnesine (g) aktar
                    g.AktifMi = dogrulanacakKullanici.AktifMi;
                    g.TcKimlikNo = dogrulanacakKullanici.TcKimlikNo;
                    g.AdSoyad = dogrulanacakKullanici.AdSoyad;
                    g.DogumYeri = dogrulanacakKullanici.DogumYeri;
                    g.DogumTarihi = dogrulanacakKullanici.DogumTarihi;
                    g.AnneAdi = dogrulanacakKullanici.AnneAdi;
                    g.BabaAdi = dogrulanacakKullanici.BabaAdi;
                    g.TelefonNo = dogrulanacakKullanici.TelefonNo;
                    g.GsmNo = dogrulanacakKullanici.GsmNo;
                    g.Email = dogrulanacakKullanici.Email;
                    g.KullaniciAdi = dogrulanacakKullanici.KullaniciAdi;
                    g.Sifre = dogrulanacakKullanici.Sifre;
                    g.KullaniciTipi = dogrulanacakKullanici.KullaniciTipi;
                    g.UnvanId = dogrulanacakKullanici.UnvanId;
                    g.BirimId = dogrulanacakKullanici.BirimId;
                    g.Tanim = dogrulanacakKullanici.Tanim;
                    g.Adres = dogrulanacakKullanici.Adres;
                    g.DigerBilgiler = dogrulanacakKullanici.DigerBilgiler;
                    g.Aciklama = dogrulanacakKullanici.Aciklama;

                    // Log kayıtlarını (metadata) güncelle
                    g.DegistirenKullanici = aktifKullanici;
                    g.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    // Veritabanına kaydet ve tabloyu yenile
                    _context.SaveChanges();
                    TabloyuYenile();
                    FormuTemizle(); // Güncelleme sonrası formu temizlemek iyi bir kullanıcı deneyimidir
                    MessageBox.Show("Güncellendi.");
                }
            }
        }

        private void dgKullanicilar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Eski metodu sildiğinizden emin olun!
            if (dgKullanicilar.SelectedItem is KullaniciModel s)
            {
                txtId.Text = s.SiraNo.ToString();
                chkAktifMi.IsChecked = s.AktifMi;
                txtTcKimlikNo.Text = s.TcKimlikNo;
                txtAdSoyad.Text = s.AdSoyad;
                txtDogumYeri.Text = s.DogumYeri;
                dpDogumTarihi.Text = s.DogumTarihi; // String ataması
                txtAnneAdi.Text = s.AnneAdi;
                txtBabaAdi.Text = s.BabaAdi;
                txtTelefonNo.Text = s.TelefonNo;
                txtGsmNo.Text = s.GsmNo;
                txtEmail.Text = s.Email;
                txtKullaniciAdi.Text = s.KullaniciAdi;
                txtSifre.Text = s.Sifre;

                cmbKullaniciTipi.Text = s.KullaniciTipi;
                cmbUnvan.SelectedValue = s.UnvanId;
                cmbBirim.SelectedValue = s.BirimId;

                txtTanim.Text = s.Tanim;
                txtAdres.Text = s.Adres;
                txtDigerBilgiler.Text = s.DigerBilgiler;
                txtAciklama.Text = s.Aciklama;

                txtKaydeden.Text = s.KaydedenKullanici;
                txtKayitTarihi.Text = s.KayitTarihi;
                txtDegistiren.Text = s.DegistirenKullanici;
                if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = s.DegistirmeTarihi;
            }
        }

        public void FormuTemizle()
        {
            txtId.Clear();
            chkAktifMi.IsChecked = false;
            txtTcKimlikNo.Clear();
            txtAdSoyad.Clear();
            txtDogumYeri.Clear();
            dpDogumTarihi.Text = string.Empty; // Text'i boşaltıyoruz
            txtAnneAdi.Clear();
            txtBabaAdi.Clear();
            txtTelefonNo.Clear();
            txtGsmNo.Clear();
            txtEmail.Clear();
            txtKullaniciAdi.Clear();
            txtSifre.Clear();

            cmbKullaniciTipi.SelectedIndex = -1;
            cmbUnvan.SelectedIndex = -1;
            cmbBirim.SelectedIndex = -1;

            txtTanim.Clear();
            txtAdres.Clear();
            txtDigerBilgiler.Clear();
            txtAciklama.Clear();

            txtKaydeden.Clear();
            txtKayitTarihi.Clear();
            txtDegistiren.Clear();
            if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Clear();

            dgKullanicilar.SelectedIndex = -1;
        }
        public void VeriyiSil()
        {
            if (dgKullanicilar.SelectedItem is KullaniciModel s)
            {
                if (MessageBox.Show("Silinsin mi?", "Onay", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _context.Kullanicilar.Remove(s);
                    _context.SaveChanges();
                    TabloyuYenile();
                    FormuTemizle(); // Silme işleminden sonra formdaki eski verileri temizler
                    MessageBox.Show("Kayıt silindi.");
                }
            }
        }

        private bool KullaniciDogrula(KullaniciModel k, string islem)
        {
            // Boş alan kontrolü (İsteğe bağlı, önemli alanları zorunlu kılmak için)
            if (string.IsNullOrWhiteSpace(k.KullaniciAdi))
            {
                MessageBox.Show("Kullanıcı Adı boş bırakılamaz!", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(k.TcKimlikNo))
            {
                MessageBox.Show("TC Kimlik No boş bırakılamaz!", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // 1. Kullanıcı Adı Benzersizlik Kontrolü
            // Eğer işlem "Ekle" ise direkt veri tabanına bak,
            // "Güncelle" ise kendisi hariç diğer kullanıcılara bak.
            bool kullaniciAdiVarMi = islem == "Ekle"
                ? _context.Kullanicilar.Any(x => x.KullaniciAdi == k.KullaniciAdi)
                : _context.Kullanicilar.Any(x => x.KullaniciAdi == k.KullaniciAdi && x.Id != k.Id);

            if (kullaniciAdiVarMi)
            {
                MessageBox.Show("Bu Kullanıcı Adı zaten kullanılıyor. Lütfen farklı bir Kullanıcı Adı belirleyin.", "Benzersizlik Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // 2. TC Kimlik No Benzersizlik Kontrolü (İsteğe bağlı ama önerilir)
            bool tcKimlikVarMi = islem == "Ekle"
                ? _context.Kullanicilar.Any(x => x.TcKimlikNo == k.TcKimlikNo)
                : _context.Kullanicilar.Any(x => x.TcKimlikNo == k.TcKimlikNo && x.Id != k.Id);

            if (tcKimlikVarMi)
            {
                MessageBox.Show("Bu TC Kimlik No ile kayıtlı başka bir kullanıcı bulunuyor.", "Benzersizlik Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        } // Basitleştirilmiş
    }
}