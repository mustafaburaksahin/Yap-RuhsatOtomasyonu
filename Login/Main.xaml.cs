using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YapıRuhsatOtomasyonu.YapiBilgileri;
using YapıRuhsatOtomasyonu.SicilBilgileri;
using YapıRuhsatOtomasyonu.KullaniciBilgileri;
using YapıRuhsatOtomasyonu.Yonetim;
using YapıRuhsatOtomasyonu.Models;
using YapıRuhsatOtomasyonu.Data;

namespace YapıRuhsatOtomasyonu.Login
{
    public class TabItemModel
    {
        public string? Header { get; set; }
        public Page? Content { get; set; }
        public string? Tag { get; set; }
    }

    public partial class Main : Window
    {
        private ObservableCollection<TabItemModel> _tabs = new ObservableCollection<TabItemModel>();

        public Main()
        {
            InitializeComponent();

            BasligiGuncelle();
            LogoYukle();

            tcPages.ItemsSource = _tabs;

            // --- NAVBAR MODÜLLERİNİ YÜKLE ---
            NavbarModulYetkileriniYukle();

            // Başlangıç Durumu: İşlem paneli gizli, Logo açık
            IslemPaneli.Visibility = Visibility.Collapsed;
            imgCenterLogo.Visibility = Visibility.Visible;

            MainFrame.Navigated += (s, e) =>
            {
                if (MainFrame.Content is Page activePage)
                {
                    activePage.Loaded += (pageSender, pageArgs) =>
                    {
                        var dg = FindVisualChild<DataGrid>(activePage);
                        if (dg != null)
                        {
                            // DÜZELTİLDİ: İlk satırı zorla seçtiren (dg.SelectedIndex = 0) mantığı kaldırıldı.
                            // Sayfa açıldığında formların boş kalması için seçim -1 (boş) olarak ayarlanıyor.
                            dg.SelectedIndex = -1;
                            UpdateNavDisplay(dg);
                        }
                        else
                        {
                            txtCurrentNav.Text = "0";
                            lblTotalNav.Text = "/ 0";
                        }
                    };
                }
            };
        }

        // --- NAVBAR MODÜL YETKİLERİ ---
        public void NavbarModulYetkileriniYukle()
        {
            var user = OturumYonetimi.GirisYapanKullanici;
            if (user == null) return;

            // Kurucu Admin Kontrolü
            if (user.KullaniciAdi == "admin" && user.Sifre == "123")
            {
                SetAllVisibility(Visibility.Visible);
                return;
            }

            using (var context = new AppDbContext())
            {
                var tumYetkiler = context.Yetkiler.Where(x => x.KullaniciAdi == user.KullaniciAdi).ToList();

                // Yapı Bilgileri
                BtnYapilar.Visibility = YetkiKontrol(tumYetkiler, "Yapılar");
                BtnBelgeTipleri.Visibility = YetkiKontrol(tumYetkiler, "Belge Tipleri");
                BtnDosyaTipleri.Visibility = YetkiKontrol(tumYetkiler, "Dosya Tipleri");
                BtnRuhsatVerilisAmaclari.Visibility = YetkiKontrol(tumYetkiler, "Ruhsat Veriliş Amacı");
                BtnIskanVerilisAmaclari.Visibility = YetkiKontrol(tumYetkiler, "İskan Veriliş Amacı");
                tabYapiBilgileri.Visibility = CheckModulVisibility(tabYapiBilgileri);

                // Sicil Bilgileri
                BtnSiciller.Visibility = YetkiKontrol(tumYetkiler, "Siciller");
                BtnSicilTipleri.Visibility = YetkiKontrol(tumYetkiler, "Sicil Tipleri");
                tabSicilBilgileri.Visibility = CheckModulVisibility(tabSicilBilgileri);

                // Kullanıcı Bilgileri
                BtnKullanicilar.Visibility = (user.KullaniciTipi == "User") ? Visibility.Collapsed : YetkiKontrol(tumYetkiler, "Kullanıcılar");
                BtnUnvanlar.Visibility = YetkiKontrol(tumYetkiler, "Ünvanlar");
                BtnBirimler.Visibility = YetkiKontrol(tumYetkiler, "Birimler");
                tabKullaniciBilgileri.Visibility = CheckModulVisibility(tabKullaniciBilgileri);

                // Yönetim
                if (user.KullaniciTipi == "User")
                {
                    tabYonetimModulu.Visibility = Visibility.Collapsed;
                }
                else
                {
                    btnAdminPaneli.Visibility = YetkiKontrol(tumYetkiler, "Admin Paneli");
                    btnProgramAyarlari.Visibility = YetkiKontrol(tumYetkiler, "Program Ayarları");
                    tabYonetimModulu.Visibility = CheckModulVisibility(tabYonetimModulu);
                }
            }
        }

        private Visibility CheckModulVisibility(TabItem tab)
        {
            var panel = tab.Content as StackPanel;
            if (panel == null) return Visibility.Collapsed;
            return panel.Children.OfType<Button>().Any(b => b.Visibility == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetAllVisibility(Visibility visibility)
        {
            tabYapiBilgileri.Visibility = tabSicilBilgileri.Visibility = tabKullaniciBilgileri.Visibility = tabYonetimModulu.Visibility = visibility;
            BtnYapilar.Visibility = BtnBelgeTipleri.Visibility = BtnDosyaTipleri.Visibility = BtnRuhsatVerilisAmaclari.Visibility = BtnIskanVerilisAmaclari.Visibility = visibility;
            BtnSiciller.Visibility = BtnSicilTipleri.Visibility = visibility;
            BtnKullanicilar.Visibility = BtnUnvanlar.Visibility = BtnBirimler.Visibility = visibility;
            btnAdminPaneli.Visibility = btnProgramAyarlari.Visibility = visibility;
        }

        private Visibility YetkiKontrol(System.Collections.Generic.List<YetkiModel> yetkiler, string panelAdi)
        {
            var yetki = yetkiler.FirstOrDefault(x => x.Panel == panelAdi);
            return (yetki != null && yetki.YetkiVerildiMi) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void IslemButonYetkileriniUygula(string panelTag)
        {
            var kadi = OturumYonetimi.GirisYapanKullanici?.KullaniciAdi;
            if (string.IsNullOrEmpty(kadi)) return;

            // Kurucu admin ise tüm butonlar daima açıktır
            if (kadi == "admin")
            {
                BtnEkle.Visibility = BtnSil.Visibility = BtnGuncelle.Visibility = Visibility.Visible;
                return;
            }

            using (var context = new AppDbContext())
            {
                var yetki = context.Yetkiler.FirstOrDefault(x => x.KullaniciAdi == kadi && x.Panel == panelTag);
                if (yetki != null)
                {
                    BtnEkle.Visibility = yetki.EkleYetki ? Visibility.Visible : Visibility.Collapsed;
                    BtnSil.Visibility = yetki.SilYetki ? Visibility.Visible : Visibility.Collapsed;
                    BtnGuncelle.Visibility = yetki.GuncelleYetki ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    BtnEkle.Visibility = BtnSil.Visibility = BtnGuncelle.Visibility = Visibility.Collapsed;
                }
            }
        }

        // --- TAB SEÇİMİ VE GÖRÜNÜRLÜK YÖNETİMİ ---
        private void tcPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tcPages.SelectedItem is TabItemModel selectedTab)
            {
                MainFrame.Content = selectedTab.Content;
                imgCenterLogo.Visibility = Visibility.Collapsed; // Sayfa açılınca logoyu gizle

                if (selectedTab.Tag == "Admin Paneli" || selectedTab.Tag == "Program Ayarları")
                {
                    IslemPaneli.Visibility = Visibility.Collapsed;
                }
                else
                {
                    IslemPaneli.Visibility = Visibility.Visible;
                    IslemButonYetkileriniUygula(selectedTab.Tag ?? "");
                }

                // DÜZELTME: Sekmeler arası geçiş yapıldığında da DataGrid seçimi otomatik dolmasın diye temizleniyor
                var dg = FindVisualChild<DataGrid>(selectedTab.Content);
                if (dg != null)
                {
                    dg.SelectedIndex = -1;
                }
            }
            e.Handled = true;
        }

        private void BtnCloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is TabItemModel tabData)
            {
                SekmeKapatmaTrafiği(tabData);
            }
        }

        private void SekmeKapatmaTrafiği(TabItemModel tabData)
        {
            int kapatilanIndex = _tabs.IndexOf(tabData);
            _tabs.Remove(tabData);

            if (_tabs.Count == 0)
            {
                MainFrame.Content = null;
                tcPages.Visibility = Visibility.Collapsed;
                IslemPaneli.Visibility = Visibility.Collapsed;
                imgCenterLogo.Visibility = Visibility.Visible;
            }
            else
            {
                int yeniIndex = Math.Min(kapatilanIndex, _tabs.Count - 1);
                tcPages.SelectedItem = _tabs[yeniIndex];
            }
        }

        // --- SAYFA AÇMA ---
        public void SayfaAc(string baslik, Page sayfa, string tag)
        {
            if ((tag == "Admin Paneli" || tag == "Program Ayarları") && OturumYonetimi.GirisYapanKullanici?.KullaniciTipi != "Admin" && OturumYonetimi.GirisYapanKullanici?.KullaniciAdi != "admin")
            {
                MessageBox.Show("Bu bölüme erişim yetkiniz bulunmamaktadır!", "Yetki Engeli", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            tcPages.Visibility = Visibility.Visible;
            var varOlan = _tabs.FirstOrDefault(t => t.Tag == tag);

            if (varOlan != null)
            {
                tcPages.SelectedItem = varOlan;
            }
            else
            {
                var yeni = new TabItemModel { Header = baslik, Content = sayfa, Tag = tag };
                _tabs.Add(yeni);
                tcPages.SelectedItem = yeni;
            }
        }

        // --- BUTON TIKLAMALARI ---
        private void BtnYapilar_Click(object sender, RoutedEventArgs e) => SayfaAc("Yapılar", new Yapilar(), "Yapılar");
        private void BtnBelgeTipleri_Click(object sender, RoutedEventArgs e) => SayfaAc("Belge Tipleri", new BelgePage(), "Belge Tipleri");
        private void BtnDosyaTipleri_Click(object sender, RoutedEventArgs e) => SayfaAc("Dosya Tipleri", new DosyaPage(), "Dosya Tipleri");
        private void BtnRuhsatVerilisAmaclari_Click(object sender, RoutedEventArgs e) => SayfaAc("Ruhsat Veriliş Amacı", new RuhsatPage(), "Ruhsat Veriliş Amacı");
        private void BtnIskanVerilisAmaclari_Click(object sender, RoutedEventArgs e) => SayfaAc("İskan Veriliş Amacı", new IskanPage(), "İskan Veriliş Amacı");
        private void BtnSiciller_Click(object sender, RoutedEventArgs e) => SayfaAc("Siciller", new SicilPage(), "Siciller");
        private void BtnSicilTipleri_Click(object sender, RoutedEventArgs e) => SayfaAc("Sicil Tipleri", new SicilTipleriPage(), "Sicil Tipleri");
        private void BtnKullanicilar_Click(object sender, RoutedEventArgs e) => SayfaAc("Kullanıcılar", new KullanicilarPage(), "Kullanıcılar");
        private void BtnUnvanlar_Click(object sender, RoutedEventArgs e) => SayfaAc("Ünvanlar", new UnvanPage(), "Ünvanlar");
        private void BtnBirimler_Click(object sender, RoutedEventArgs e) => SayfaAc("Birimler", new BirimPage(), "Birimler");
        private void BtnAdminPaneli_Click(object sender, RoutedEventArgs e) => SayfaAc("Admin Paneli", new AdminPaneli(), "Admin Paneli");
        private void BtnProgramAyarlari_Click(object sender, RoutedEventArgs e) => SayfaAc("Program Ayarları", new Program(), "Program Ayarları");

        private void BtnEkle_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is BelgePage belge) belge.VeriyiTabloyaEkle();
            else if (MainFrame.Content is DosyaPage dosya) dosya.VeriyiTabloyaEkle();
            else if (MainFrame.Content is RuhsatPage ruhsat) ruhsat.VeriyiTabloyaEkle();
            else if (MainFrame.Content is IskanPage iskan) iskan.VeriyiTabloyaEkle();
            else if (MainFrame.Content is SicilPage sicil) sicil.VeriyiTabloyaEkle();
            else if (MainFrame.Content is SicilTipleriPage sTipi) sTipi.VeriyiTabloyaEkle();
            else if (MainFrame.Content is KullanicilarPage kullanici) kullanici.VeriyiTabloyaEkle();
            else if (MainFrame.Content is Yapilar yapi) yapi.VeriyiTabloyaEkle();
            else if (MainFrame.Content is UnvanPage unvan) unvan.VeriyiTabloyaEkle();
            else if (MainFrame.Content is BirimPage birim) birim.VeriyiTabloyaEkle();
        }

        private void BtnSil_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is BelgePage belge) belge.VeriyiSil();
            else if (MainFrame.Content is DosyaPage dosya) dosya.VeriyiSil();
            else if (MainFrame.Content is RuhsatPage ruhsat) ruhsat.VeriyiSil();
            else if (MainFrame.Content is IskanPage iskan) iskan.VeriyiSil();
            else if (MainFrame.Content is SicilPage sicil) sicil.VeriyiSil();
            else if (MainFrame.Content is SicilTipleriPage sTipi) sTipi.VeriyiSil();
            else if (MainFrame.Content is KullanicilarPage kullanici) kullanici.VeriyiSil();
            else if (MainFrame.Content is Yapilar yapi) yapi.VeriyiSil();
            else if (MainFrame.Content is UnvanPage unvan) unvan.VeriyiSil();
            else if (MainFrame.Content is BirimPage birim) birim.VeriyiSil();
        }

        private void BtnGuncelle_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is BelgePage belge) belge.VeriyiGuncelle();
            else if (MainFrame.Content is DosyaPage dosya) dosya.VeriyiGuncelle();
            else if (MainFrame.Content is RuhsatPage ruhsat) ruhsat.VeriyiGuncelle();
            else if (MainFrame.Content is IskanPage iskan) iskan.VeriyiGuncelle();
            else if (MainFrame.Content is SicilPage sicil) sicil.VeriyiGuncelle();
            else if (MainFrame.Content is SicilTipleriPage sTipi) sTipi.VeriyiGuncelle();
            else if (MainFrame.Content is KullanicilarPage kullanici) kullanici.VeriyiGuncelle();
            else if (MainFrame.Content is Yapilar yapi) yapi.VeriyiGuncelle();
            else if (MainFrame.Content is UnvanPage unvan) unvan.VeriyiGuncelle();
            else if (MainFrame.Content is BirimPage birim) birim.VeriyiGuncelle();
        }

        private void BtnNavigation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && MainFrame.Content is DependencyObject current)
            {
                var dg = FindVisualChild<DataGrid>(current);
                if (dg == null || dg.Items.Count == 0) return;

                string? dir = btn.Tag?.ToString();
                switch (dir)
                {
                    case "First": dg.SelectedIndex = 0; break;
                    case "Last": dg.SelectedIndex = dg.Items.Count - 1; break;
                    case "Prev": if (dg.SelectedIndex > 0) dg.SelectedIndex--; break;
                    case "Next": if (dg.SelectedIndex < dg.Items.Count - 1) dg.SelectedIndex++; break;
                }
                dg.ScrollIntoView(dg.SelectedItem);
                UpdateNavDisplay(dg);
            }
        }

        public void UpdateNavDisplay(DataGrid dg)
        {
            int index = dg.SelectedIndex + 1;
            txtCurrentNav.Text = index > 0 ? index.ToString() : "0";
            lblTotalNav.Text = "/ " + dg.Items.Count.ToString();
        }

        private T? FindVisualChild<T>(DependencyObject? obj) where T : DependencyObject
        {
            if (obj == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t) return t;
                T? childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null) return childOfChild;
            }
            return null;
        }

        private void BtnKapat_Click(object sender, RoutedEventArgs e)
        {
            if (tcPages.SelectedItem is TabItemModel selectedTab)
            {
                SekmeKapatmaTrafiği(selectedTab);
            }
        }

        public void BasligiGuncelle()
        {
            using (var db = new AppDbContext())
            {
                var s = db.SistemAyarlari.FirstOrDefault();
                if (s != null && !string.IsNullOrEmpty(s.SistemAdi)) this.Title = s.SistemAdi;
            }
        }

        public void LogoYukle()
        {
            using (var db = new AppDbContext())
            {
                var s = db.SistemAyarlari.FirstOrDefault();
                if (s != null && !string.IsNullOrWhiteSpace(s.LogoYolu) && System.IO.File.Exists(s.LogoYolu))
                {
                    var b = new BitmapImage();
                    using (FileStream fs = new FileStream(s.LogoYolu, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        b.BeginInit();
                        b.StreamSource = fs;
                        b.CacheOption = BitmapCacheOption.OnLoad;
                        b.EndInit();
                    }
                    b.Freeze();
                    this.Icon = b;
                    imgCenterLogo.Source = b;
                }
            }
        }
    }
}