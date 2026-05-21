using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using YapıRuhsatOtomasyonu.Data;
using YapıRuhsatOtomasyonu.Models;
using YapıRuhsatOtomasyonu.Login;
using System.IO.Compression;

namespace YapiRuhsatOtomasyonu.Pages
{
    public partial class DosyaDetay : Window
    {
        private DosyaDetayModel _currentModel;
        private int _gelenYapiId;
        private readonly AppDbContext _context = new AppDbContext();

        // Doğrudan ortak "Arsiv" klasörüne yönlendirme yolu
        private readonly string _guvenliAnaKlasor = @"C:\YapiRuhsatArsiv\Arsiv";

        public DosyaDetay(DosyaDetayModel? model = null, string? secilenYol = null, int yapiId = 0, int gorunenSiraNo = 0)
        {
            InitializeComponent();
            _gelenYapiId = yapiId;

            VerileriYukle();
            cmbDosyaTipi.SelectionChanged += SecimDegisti;

            if (model != null)
            {
                _currentModel = model;
                VerileriFormaDoldur(gorunenSiraNo);
            }
            else
            {
                _currentModel = new DosyaDetayModel { YapiSiraNo = yapiId };

                // TextBox'a DataGrid'deki Sıra No'yu yazıyoruz
                txtId.Text = gorunenSiraNo.ToString();

                _currentModel.KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                YapiSahibiGetir();

                if (!string.IsNullOrEmpty(secilenYol))
                {
                    txtDosyaYolu.Text = Path.GetFileName(secilenYol);
                    txtDosyaKonumu.Text = Path.Combine(_guvenliAnaKlasor, yapiId.ToString());
                    _currentModel.DosyaYolu = secilenYol;
                }
            }
        }

        private void VerileriFormaDoldur(int gorunenSiraNo)
        {
            if (_currentModel == null) return;

            txtId.Text = gorunenSiraNo.ToString();
            txtDosyaKonumu.Text = !string.IsNullOrEmpty(_currentModel.DosyaYolu) ? Path.GetDirectoryName(_currentModel.DosyaYolu) : "";
            txtDosyaYolu.Text = Path.GetFileName(_currentModel.DosyaYolu ?? "");
            cmbDosyaTipi.SelectedValue = _currentModel.DosyaTipId;
            txtAciklama.Text = _currentModel.Aciklama;

            YapiSahibiGetir();
        }

        private void VerileriYukle()
        {
            try
            {
                cmbDosyaTipi.ItemsSource = _context.DosyaTipleri.OrderBy(x => x.Ad).ToList();
                cmbDosyaTipi.DisplayMemberPath = "Ad";
                cmbDosyaTipi.SelectedValuePath = "Id";
            }
            catch (Exception ex) { MessageBox.Show("Veriler yüklenemedi: " + ex.Message); }
        }

        private void YapiSahibiGetir()
        {
            var yapi = _context.Yapilar.FirstOrDefault(y => y.Id == _gelenYapiId);
            if (yapi != null)
            {
                txtYapiSahibi.Text = yapi.YapiSahibi;
            }
        }

        private void SecimDegisti(object sender, SelectionChangedEventArgs e) => GuncelKonumuTetikle();

        private void GuncelKonumuTetikle()
        {
            string klasorYolu = Path.Combine(_guvenliAnaKlasor, _gelenYapiId.ToString());
            txtDosyaKonumu.Text = klasorYolu;
        }

        private void BtnKaydet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbDosyaTipi.SelectedValue == null)
                {
                    MessageBox.Show("Lütfen bir Dosya Tipi seçiniz!", "Eksik Bilgi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_gelenYapiId <= 0)
                {
                    MessageBox.Show("Yapı seçimi hatalı!", "Hata");
                    return;
                }

                string hedefKlasor = txtDosyaKonumu.Text.Trim();
                if (string.IsNullOrEmpty(hedefKlasor))
                    hedefKlasor = Path.Combine(_guvenliAnaKlasor, _gelenYapiId.ToString());

                if (!Directory.Exists(hedefKlasor))
                    Directory.CreateDirectory(hedefKlasor);

                string dosyaAdi = txtDosyaYolu.Text.Trim();
                if (string.IsNullOrEmpty(dosyaAdi)) return;

                // GÜNCELLEME: Boyut veya checkbox fark etmeksizin tüm uzantılar .zip formatına zorlanıyor
                string zipDosyaAdi = Path.GetFileNameWithoutExtension(dosyaAdi) + ".zip";
                string hedefTamYol = Path.Combine(hedefKlasor, zipDosyaAdi);
                string? kaynakYol = _currentModel.DosyaYolu;

                if (!string.IsNullOrEmpty(kaynakYol) && File.Exists(kaynakYol) && kaynakYol != hedefTamYol)
                {
                    // Eğer hedefte aynı isimde bir zip varsa çakışmayı önlemek için siliyoruz
                    if (File.Exists(hedefTamYol)) File.Delete(hedefTamYol);

                    // GÜNCELLEME: Dosya doğrudan otomatik olarak sıkıştırılarak zip içine aktarılıyor
                    using (ZipArchive archive = ZipFile.Open(hedefTamYol, ZipArchiveMode.Create))
                    {
                        archive.CreateEntryFromFile(kaynakYol, Path.GetFileName(kaynakYol));
                    }

                    _currentModel.DosyaYolu = hedefTamYol;
                }

                _currentModel.YapiSiraNo = _gelenYapiId;
                _currentModel.DosyaTipId = cmbDosyaTipi.SelectedValue as int?;
                _currentModel.Aciklama = txtAciklama.Text;
                _currentModel.DigerBilgiler = string.Empty;

                string aktifKullanici = OturumYonetimi.GirisYapanKullanici?.KullaniciAdi ?? "Bilinmeyen Kullanıcı";

                if (_currentModel.SiraNo <= 0)
                {
                    _currentModel.KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                    _currentModel.KaydedenKullanici = aktifKullanici;
                    _context.DosyaDetaylar.Add(_currentModel);
                }
                else
                {
                    var local = _context.DosyaDetaylar.Local.FirstOrDefault(x => x.SiraNo == _currentModel.SiraNo);
                    if (local != null)
                        _context.Entry(local).State = EntityState.Detached;

                    _context.Entry(_currentModel).State = EntityState.Modified;
                }

                _context.SaveChanges();
                MessageBox.Show("Dosya otomatik olarak sıkıştırıldı ve Arşiv klasörüne kaydedildi.", "Başarılı");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kaydedilemedi: " + (ex.InnerException?.Message ?? ex.Message), "Hata");
            }
        }

        private void BtnKapat_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}