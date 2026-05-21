using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Login;
using YapıRuhsatOtomasyonu.Models;
using YapiRuhsatOtomasyonu.Pages;
using YapıRuhsatOtomasyonu.Pages;
using Amazon.S3.Model;

namespace YapıRuhsatOtomasyonu.YapiBilgileri
{
    public partial class Yapilar : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        // Sayfa genelinde kullanıcının yetkisini tutacak global değişken
        private bool _islemYetkisiVar = false;

        // Loglama için aktif kullanıcının Ad Soyad bilgisi sınıf seviyesine alındı
        string aktifKullaniciAdSoyad = OturumYonetimi.GirisYapanKullanici?.AdSoyad ?? "Bilinmeyen Kullanıcı";

        // Dinamik Sütun Filtrelemesi için CollectionView tanımı
        private ICollectionView _yapilarView = null!;

        // GÜNCELLEME: Hangi TextBox'a ne yazıldığını hafızada tutacak sözlük
        private Dictionary<string, string> _filtreler = new Dictionary<string, string>();

        public Yapilar()
        {
            InitializeComponent();
            TabloyuYenile();

            // Sayfa yüklendiğinde Yetki Kontrolü
            this.Loaded += Page_Loaded;
        }

        // --- YETKİ KONTROLÜ VE GÖRÜNÜRLÜK ---
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = OturumYonetimi.GirisYapanKullanici;
            if (user == null) return; 

            if (user.KullaniciAdi == "admin" || user.KullaniciTipi == "Yönetici")
            {
                _islemYetkisiVar = true;
                if (Panel1 != null) Panel1.Visibility = Visibility.Visible;
                SetDataGridActionColumnsVisibility(Visibility.Visible);
                return;
            }

            var yetki = _context.Yetkiler.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi && x.Panel == "Yapılar");

            _islemYetkisiVar = yetki != null && (yetki.EkleYetki || yetki.GuncelleYetki);

            if (Panel1 != null) Panel1.Visibility = _islemYetkisiVar ? Visibility.Visible : Visibility.Collapsed;
            SetDataGridActionColumnsVisibility(_islemYetkisiVar ? Visibility.Visible : Visibility.Collapsed);
        }

        private void SetDataGridActionColumnsVisibility(Visibility visibility)
        {
            var colBelge = dgYapilar.Columns.FirstOrDefault(c => c.Header?.ToString() == "Belge Ekle");
            var colDosya = dgYapilar.Columns.FirstOrDefault(c => c.Header?.ToString() == "Dosya Ekle");

            if (colBelge != null) colBelge.Visibility = visibility;
            if (colDosya != null) colDosya.Visibility = visibility;
        }

        // --- VERİ YÜKLEME VE TABLO İŞLEMLERİ ---
        private void dgYapilar_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void TabloyuYenile()
        {
            try
            {
                var liste = _context.Yapilar.ToList();

                for (int i = 0; i < liste.Count; i++)
                {
                    liste[i].SiraNo = i + 1;
                }

                // Event tetiklenmesini önlemek için önce SelectedIndex sıfırlanıyor
                dgYapilar.SelectedIndex = -1;
                dgYapilar.ItemsSource = null;

                // Listeyi doğrudan bağlamak yerine CollectionViewSource üzerinden süzgeç mimarisine alıyoruz
                _yapilarView = CollectionViewSource.GetDefaultView(liste);
                _yapilarView.Filter = YapilarFiltreMantigi;
                dgYapilar.ItemsSource = _yapilarView;

                // Yapı Sahibi Combobox (Sicillerden çekilir)
                var sicilListesi = _context.Siciller
                    .Where(s => s.AdSoyad != null && s.TcKimlikNo != null)
                    .Select(s => new { s.TcKimlikNo, s.AdSoyad })
                    .Distinct()
                    .ToList()
                    .Select(x => new
                    {
                        TCKimlikNo = x.TcKimlikNo,
                        GorunenAd = $"{x.AdSoyad} - {x.TcKimlikNo}"
                    })
                    .ToList();

                cmbYapiSahibi.ItemsSource = sicilListesi;
                cmbYapiSahibi.DisplayMemberPath = "GorunenAd";
                cmbYapiSahibi.SelectedValuePath = "TCKimlikNo";

                if (Window.GetWindow(this) is Main anaPencere)
                {
                    anaPencere.UpdateNavDisplay(dgYapilar);
                }

                // Formun temiz gelmesini garanti altına alıyoruz
                FormuTemizle();
                dgYapilar.SelectedIndex = -1; // ItemsSource bağlandıktan sonra seçimi tekrar boşa çıkarıyoruz
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tablo yenilenirken hata: " + ex.Message);
            }
        }

        // GÜNCELLEME: Kutulara her yazıldığında metni hafızaya (Sözlüğe) alır ve filtrelemeyi tetikler
        private void txtFiltre_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                // Hangi TextBox ise onun Name'ini anahtar olarak alıp yazdığın yazıyı kaydediyoruz
                _filtreler[tb.Name] = tb.Text.Trim();

                if (_yapilarView != null)
                {
                    _yapilarView.Refresh(); // Her harf girildiğinde veya silindiğinde tabloyu anlık yeniler
                    dgYapilar.SelectedIndex = -1; // Kayıt seçimi süzme işleminde kaybolacağı için formları güvenle temizle
                }
            }
        }

        // GÜNCELLEME: Filtreleme mantığı artık UI'dan değil, doğrudan bellekten (Sözlükten) çalışıyor
        private bool YapilarFiltreMantigi(object item)
        {
            if (!(item is YapiModel yapi)) return false;

            if (_filtreler.TryGetValue("txtFiltreId", out string? fId) && !string.IsNullOrWhiteSpace(fId))
                if (!yapi.SiraNo.ToString().Contains(fId)) return false;

            if (_filtreler.TryGetValue("txtFiltreYapiSahibi", out string? fSahibi) && !string.IsNullOrWhiteSpace(fSahibi))
                if (yapi.YapiSahibi == null || !yapi.YapiSahibi.Contains(fSahibi, StringComparison.OrdinalIgnoreCase)) return false;

            if (_filtreler.TryGetValue("txtFiltreYapiKimlikNo", out string? fKimlik) && !string.IsNullOrWhiteSpace(fKimlik))
                if (yapi.YapiKimlikNo == null || !yapi.YapiKimlikNo.Contains(fKimlik, StringComparison.OrdinalIgnoreCase)) return false;

            if (_filtreler.TryGetValue("txtFiltrePafta", out string? fPafta) && !string.IsNullOrWhiteSpace(fPafta))
                if (yapi.PaftaNo == null || !yapi.PaftaNo.Contains(fPafta, StringComparison.OrdinalIgnoreCase)) return false;

            if (_filtreler.TryGetValue("txtFiltreAda", out string? fAda) && !string.IsNullOrWhiteSpace(fAda))
                if (yapi.Ada == null || !yapi.Ada.Contains(fAda, StringComparison.OrdinalIgnoreCase)) return false;

            if (_filtreler.TryGetValue("txtFiltreParsel", out string? fParsel) && !string.IsNullOrWhiteSpace(fParsel))
                if (yapi.Parsel == null || !yapi.Parsel.Contains(fParsel, StringComparison.OrdinalIgnoreCase)) return false;

            if (_filtreler.TryGetValue("txtFiltreMahalle", out string? fMahalle) && !string.IsNullOrWhiteSpace(fMahalle))
                if (yapi.Mahalle == null || !yapi.Mahalle.Contains(fMahalle, StringComparison.OrdinalIgnoreCase)) return false;

            if (_filtreler.TryGetValue("txtFiltreCadde", out string? fCadde) && !string.IsNullOrWhiteSpace(fCadde))
                if (yapi.Cadde == null || !yapi.Cadde.Contains(fCadde, StringComparison.OrdinalIgnoreCase)) return false;

            if (_filtreler.TryGetValue("txtFiltreSokak", out string? fSokak) && !string.IsNullOrWhiteSpace(fSokak))
                if (yapi.Sokak == null || !yapi.Sokak.Contains(fSokak, StringComparison.OrdinalIgnoreCase)) return false;

            return true; // Tüm uyan kriterler tabroda gösterilmeye hak kazanır
        }

        // --- CRUD İŞLEMLERİ ---
        public void VeriyiTabloyaEkle()
        {
            if (string.IsNullOrWhiteSpace(txtAda.Text) || string.IsNullOrWhiteSpace(txtParsel.Text))
            {
                MessageBox.Show("Ada ve Parsel zorunludur!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var yeniYapi = new YapiModel
            {
                YapiSahibi = cmbYapiSahibi.Text,
                YapiKimlikNo = txtYapiKimlikNo.Text,
                PaftaNo = txtPafta.Text,
                Ada = txtAda.Text,
                Parsel = txtParsel.Text,
                Mahalle = txtMahalle.Text,
                Cadde = txtCadde.Text,
                Sokak = txtSokak.Text,
                DigerBilgiler = txtDigerBilgiler.Text,
                Aciklama = txtAciklama.Text,

                KaydedenKullanici = aktifKullaniciAdSoyad,
                KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
            };

            _context.Yapilar.Add(yeniYapi);
            _context.SaveChanges();

            TabloyuYenile();
            MessageBox.Show("Kayıt eklendi.");
        }

        public void VeriyiGuncelle()
        {
            if (dgYapilar.SelectedItem is YapiModel secilenKayit)
            {
                var kayit = _context.Yapilar.Find(secilenKayit.Id);
                if (kayit != null)
                {
                    kayit.YapiSahibi = cmbYapiSahibi.Text;
                    kayit.YapiKimlikNo = txtYapiKimlikNo.Text;
                    kayit.PaftaNo = txtPafta.Text;
                    kayit.Ada = txtAda.Text;
                    kayit.Parsel = txtParsel.Text;
                    kayit.Mahalle = txtMahalle.Text;
                    kayit.Cadde = txtCadde.Text;
                    kayit.Sokak = txtSokak.Text;
                    kayit.DigerBilgiler = txtDigerBilgiler.Text;
                    kayit.Aciklama = txtAciklama.Text;

                    kayit.DegistirenKullanici = aktifKullaniciAdSoyad;
                    kayit.DegistirmeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    if (txtDegistiren != null) txtDegistiren.Text = kayit.DegistirenKullanici;
                    if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = kayit.DegistirmeTarihi;

                    _context.SaveChanges();
                    TabloyuYenile();
                    MessageBox.Show("Güncellendi.");
                }
            }
        }

        public void VeriyiSil()
        {
            if (dgYapilar.SelectedItem is YapiModel s)
            {
                if (MessageBox.Show("Silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var silinecek = _context.Yapilar.Find(s.Id);
                    if (silinecek != null)
                    {
                        _context.Yapilar.Remove(silinecek);
                        _context.SaveChanges();
                        TabloyuYenile();
                    }
                }
            }
        }

        // --- FORM TEMİZLEME ---
        public void FormuTemizle()
        {
            if (Panel1.Content is Panel icPanel)
            {
                foreach (var child in icPanel.Children)
                {
                    if (child is TextBox t) t.Clear();
                    if (child is ComboBox c) c.SelectedIndex = -1;

                    if (child is Panel altPanel)
                    {
                        foreach (var altChild in altPanel.Children)
                        {
                            if (altChild is TextBox at) at.Clear();
                            if (altChild is ComboBox ac) ac.SelectedIndex = -1;
                        }
                    }
                }
            }

            if (txtKaydeden != null) txtKaydeden.Clear();
            if (txtKayitTarihi != null) txtKayitTarihi.Clear();
            if (txtDegistiren != null) txtDegistiren.Clear();
            if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Clear();

            dgYapilar.SelectedItem = null;
            dgYapilar.SelectedIndex = -1; // Seçim indeksini kesin olarak sıfırlıyoruz
        }

        private void dgYapilar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Eğer seçim boşaltıldıysa (null ise) metottan çık, formları doldurma
            if (dgYapilar.SelectedItem == null) return;

            if (dgYapilar.SelectedItem is YapiModel s)
            {
                txtId.Text = s.SiraNo.ToString();
                cmbYapiSahibi.Text = s.YapiSahibi;
                txtYapiKimlikNo.Text = s.YapiKimlikNo;
                txtPafta.Text = s.PaftaNo;
                txtAda.Text = s.Ada;
                txtParsel.Text = s.Parsel;
                txtMahalle.Text = s.Mahalle;
                txtCadde.Text = s.Cadde;
                txtSokak.Text = s.Sokak;
                txtDigerBilgiler.Text = s.DigerBilgiler;
                txtAciklama.Text = s.Aciklama;

                if (txtKaydeden != null) txtKaydeden.Text = s.KaydedenKullanici;
                if (txtKayitTarihi != null) txtKayitTarihi.Text = s.KayitTarihi;
                if (txtDegistiren != null) txtDegistiren.Text = s.DegistirenKullanici;
                if (txtDegistirmeTarihi != null) txtDegistirmeTarihi.Text = s.DegistirmeTarihi;

                if (Window.GetWindow(this) is Main ana) ana.UpdateNavDisplay(dgYapilar);
            }
        }

        // --- ARŞİV VE BELGE İŞLEMLERİ ---
        // --- ARŞİV VE BELGE İŞLEMLERİ ---
        private void BtnYapininBelgeleri_Click(object sender, RoutedEventArgs e)
        {
            if (dgYapilar.SelectedItem is YapiModel secilenYapi && secilenYapi.Id > 0)
            {
                if (_context.BelgeDetay.Any(x => x.YapiSiraNo == secilenYapi.Id))
                {
                    // HATA BURADAYDI: Süsle parantez ataması (Object Initializer) düzeltildi 
                    // ve fazladan olan win.ShowDialog() silindi.
                    BelgeListesi win = new BelgeListesi(secilenYapi.Id)
                    {
                        Owner = Window.GetWindow(this)
                    };

                    win.ShowDialog(); // Sadece bir kere çağrılmalı
                }
                else
                {
                    MessageBox.Show("Arşiv boş.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnYapininDosyalari_Click(object sender, RoutedEventArgs e)
        {
            if (dgYapilar.SelectedItem is YapiModel secilenYapi && secilenYapi.Id > 0)
            {
                if (_context.DosyaDetaylar.Any(x => x.YapiSiraNo == secilenYapi.Id))
                {
                    // HATA DÜZELTİLDİ: Süsle parantez karmaşası giderildi
                    DosyaListesi win = new DosyaListesi(secilenYapi.Id, secilenYapi.SiraNo);
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Teknik dosya arşivi boş.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnBelgeEkle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is YapiModel secilen)
            {
                OpenFileDialog ofd = new OpenFileDialog { Filter = "PDF|*.pdf|Docx|*.docx|Tüm Dosyalar|*.*" };
                if (ofd.ShowDialog() == true)
                {
                    BelgeDetay win = new BelgeDetay(secilen, secilen.SiraNo, ofd.FileName);
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                    TabloyuYenile();
                }
            }
        }

        private void BtnDosyaEkle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is YapiModel secilen)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == true)
                {
                    DosyaDetay win = new DosyaDetay(null, ofd.FileName, secilen.Id, secilen.SiraNo);
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                    TabloyuYenile();
                }
            }
        }
    }
}