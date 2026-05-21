using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Windows.Xps.Serialization;

namespace YapıRuhsatOtomasyonu.Models
{
    // --- 1. BELGE TİPLERİ ---
    public class BelgeTipiModel
    {
        [NotMapped]
        public int SiraNo { get; set; }
        public int Id { get; set; }
        public string? Ad { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? Aciklama { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? KayitTarihi { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirmeTarihi { get; set; }
    }

    // --- 2. DOSYA TİPLERİ ---
    public class DosyaTipiModel
    {
        [NotMapped]
        public int SiraNo { get; set; }
        public int Id { get; set; }
        public string? Ad { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? Aciklama { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? KayitTarihi { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirmeTarihi { get; set; }
    }

    // --- 3. RUHSAT AMAÇLARI ---
    public class RuhsatAmaciModel
    {
        [NotMapped]
        public int SiraNo { get; set; }
        public int Id { get; set; }
        public string? VerilisAmaci { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? Aciklama { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? KayitTarihi { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirmeTarihi { get; set; }
    }

    // --- 4. İSKAN AMAÇLARI ---
    public class IskanAmaciModel
    {
        [NotMapped]
        public int SiraNo { get; set; }
        public int Id { get; set; }
        public string? VerilisAmaci { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? Aciklama { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? KayitTarihi { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirmeTarihi { get; set; }
    }

    // --- 5. SİCİLLER ---
    public class SicilTipiModel
    {
        [NotMapped]
        public int SiraNo { get; set; }
        public int Id { get; set; }
        public string? SicilTipi { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? Aciklama { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? KayitTarihi { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirmeTarihi { get; set; }
    }

    public class SicilModel
    {
        [NotMapped]
        public int SiraNo { get; set; }
        public int Id { get; set; }

        // DÜZELTİLEN KISIM: SicilTipiId yerine orijinal ad olan SicilTipi kullanıldı
        public int? SicilTipi { get; set; }
        [ForeignKey("SicilTipi")]
        public virtual SicilTipiModel? SicilTipiNavigation { get; set; }

        public string? TcKimlikNo { get; set; }
        public string? VergiNo { get; set; }
        public string? FirmaAdi { get; set; }
        public string? AdSoyad { get; set; }
        public string? DogumYeri { get; set; }
        public string? DogumTarihi { get; set; }
        public string? VergiDairesi { get; set; }
        public string? AnneAdi { get; set; }
        public string? BabaAdi { get; set; }
        public string? TelefonNo { get; set; }
        public string? Email { get; set; }
        public string? AlanKisi { get; set; }
        public string? Adres { get; set; }
        public string? Aciklama { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? KayitTarihi { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirmeTarihi { get; set; }
    }

    public class SistemAyarModel
    {
        [Key]
        public int Id { get; set; }
        public string? SistemAdi { get; set; }
        public string? BelediyeAdi { get; set; }
        public string? Il { get; set; }
        public string? Ilce { get; set; }
        public string? LogoYolu { get; set; }
    }

    // --- 6. KULLANICILAR ---
    public class KullaniciModel
    {
        [NotMapped]
        public int SiraNo { get; set; }
        public int Id { get; set; }
        public bool AktifMi { get; set; }
        public string? TcKimlikNo { get; set; }
        public string? AdSoyad { get; set; }
        public string? DogumYeri { get; set; }
        public string? DogumTarihi { get; set; }
        public string? AnneAdi { get; set; }
        public string? BabaAdi { get; set; }
        public string? TelefonNo { get; set; }
        public string? GsmNo { get; set; }
        public string? Email { get; set; }
        public string? KullaniciAdi { get; set; }
        public string? Sifre { get; set; }
        public string? KullaniciTipi { get; set; }

        // İlişkiler: Yetki, Unvan, Birim
        public int? YetkiId { get; set; }
        [ForeignKey("YetkiId")]
        public virtual YetkiModel? Yetki { get; set; }

        public int? UnvanId { get; set; }
        [ForeignKey("UnvanId")]
        public virtual UnvanModel? Unvan { get; set; }

        public int? BirimId { get; set; }
        [ForeignKey("BirimId")]
        public virtual BirimModel? Birim { get; set; }

        public string? Tanim { get; set; }
        public string? KullaniciBirimi { get; set; }
        public string? Adres { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? Aciklama { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? KayitTarihi { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirmeTarihi { get; set; }
    }

    // --- 7. YAPILAR ---
    public class YapiModel
    {
        [NotMapped]
        public int SiraNo { get; set; }
        public int Id { get; set; }
        public string? YapiSahibi { get; set; }
        public string? YapiKimlikNo { get; set; }
        public string? Ada { get; set; }
        public string? Parsel { get; set; }
        public string? Mahalle { get; set; }
        public string? Cadde { get; set; }
        public string? Sokak { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? Aciklama { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? KayitTarihi { get; set; }
        public string? PaftaNo { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirmeTarihi { get; set; }
    }

    // --- 8. ÜNVANLAR ---
    public class UnvanModel
    {
        [NotMapped]
        public int SiraNo { get; set; }
        public int Id { get; set; }
        public string? UnvanAdi { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? Aciklama { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? KayitTarihi { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirmeTarihi { get; set; }
    }

    // --- 9. BİRİMLER ---
    public class BirimModel
    {
        [NotMapped]
        public int SiraNo { get; set; }
        public int Id { get; set; }
        public string? BirimAdi { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? Aciklama { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? KayitTarihi { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirmeTarihi { get; set; }   
    }

    // --- 10. YETKİLER ---
    public class YetkiModel
    {
        [NotMapped]
        public int SiraNo { get; set; }

        [Key]
        public int Id { get; set; }

        public string? KullaniciAdi { get; set; }
        public string? Modul { get; set; }
        public string? Panel { get; set; }

        public bool YetkiVerildiMi { get; set; }
        public bool EkleYetki { get; set; }
        public bool SilYetki { get; set; }
        public bool GuncelleYetki { get; set; }

        public string? KayitTarihi { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirmeTarihi { get; set; }
    }

    public class YetkiTipiModel
    {
        [NotMapped]
        public int SiraNo { get; set; }
        public int Id { get; set; }
        public string? YetkiTipiAdi { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? Aciklama { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? KayitTarihi { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirmeTarihi { get; set; }
    }

    // --- DOSYA VE BELGE MODELLERİ ---
    public class DosyaModel
    {
        public int Id { get; set; }
        public int YapiSiraNo { get; set; }
        public string? DosyaAdi { get; set; }
        public string? DosyaYolu { get; set; }
        public string? Uzanti { get; set; }
        public string? KayitTarihi { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirenTarihi { get; set; }
    }

    public class BelgeModel
    {
        public int Id { get; set; }
        public int YapiSiraNo { get; set; }
        public string? BelgeAdi { get; set; }
        public string? BelgeYolu { get; set; }
        public string? Uzanti { get; set; }
        public string? KayitTarihi { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? DegistirenKullanici { get; set; }
        public string? DegistirenTarihi { get; set; }
    }

    [Table("DosyaDetay")]
    public class DosyaDetayModel
    {
        [Key]
        public int SiraNo { get; set; }
        public int YapiSiraNo { get; set; }

        [NotMapped]
        public string DosyaTipAd => DosyaTipi?.Ad ?? "Belirtilmemiş";

        public int? DosyaTipId { get; set; }
        public string? DosyaYolu { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? Aciklama { get; set; }
        public string? KayitTarihi { get; set; }
        public string? KaydedenKullanici { get; set; }

        [ForeignKey("DosyaTipId")]
        public virtual DosyaTipiModel? DosyaTipi { get; set; }
    }

    [Table("BelgeDetaylar")]
    public class BelgeDetayModel
    {
        [Key]
        public int SiraNo { get; set; }

        public int YapiSiraNo { get; set; }
        [ForeignKey("YapiSiraNo")]
        public virtual YapiModel? Yapi { get; set; }

        // İlişkiler: İskan ve Ruhsat (String alanlar int yapıldı)
        public int? RuhsatVerilisAmaciId { get; set; }
        [ForeignKey("RuhsatVerilisAmaciId")]
        public virtual RuhsatAmaciModel? RuhsatAmaci { get; set; }

        public int? IskanVerilisAmaciId { get; set; }
        [ForeignKey("IskanVerilisAmaciId")]
        public virtual IskanAmaciModel? IskanAmaci { get; set; }

        public string? RuhsatNo { get; set; }
        public string? YapiKimlikNo { get; set; }
        public string? RuhsatBilgileri { get; set; }
        public string? RuhsatTarihi { get; set; }
        public string? ParselinAlani { get; set; }
        public string? InsaatHarci { get; set; }
        public string? YapiDenetimFirmasi { get; set; }
        public string? SantiyeSefi { get; set; }
        public string? Muteahhit { get; set; }
        public string? IskanTarihi { get; set; }
        public string? BelgeyeEsasRuhsat { get; set; }
        public string? IskanNo { get; set; }
        public string? BelgeAdi { get; set; }
        public string? BelgeYolu { get; set; }
        public string? DigerBilgiler { get; set; }
        public string? Aciklama { get; set; }
        public string? KaydedenKullanici { get; set; }
        public string? KayitTarihi { get; set; }
    }

    public static class OturumYonetimi
    {
        public static KullaniciModel? GirisYapanKullanici { get; set; }

        public static void OturumuKapat()
        {
            GirisYapanKullanici = null;
        }
    }

    // --- GEÇİCİ VERİ ---
    public static class GeciciVeri
    {
        public static ObservableCollection<BelgeTipiModel> BelgeTipleriListesi { get; set; } = new ObservableCollection<BelgeTipiModel>();
        public static ObservableCollection<DosyaTipiModel> DosyaTipleriListesi { get; set; } = new ObservableCollection<DosyaTipiModel>();
        public static ObservableCollection<RuhsatAmaciModel> RuhsatAmaclariListesi { get; set; } = new ObservableCollection<RuhsatAmaciModel>();
        public static ObservableCollection<IskanAmaciModel> IskanAmaclariListesi { get; set; } = new ObservableCollection<IskanAmaciModel>();
        public static ObservableCollection<SicilModel> SicillerListesi { get; set; } = new ObservableCollection<SicilModel>();
        public static ObservableCollection<SicilTipiModel> SicilTipleriListesi { get; set; } = new ObservableCollection<SicilTipiModel>();
        public static ObservableCollection<KullaniciModel> KullanicilarListesi { get; set; } = new ObservableCollection<KullaniciModel>();
        public static ObservableCollection<YapiModel> YapilarListesi { get; set; } = new ObservableCollection<YapiModel>();
        public static ObservableCollection<UnvanModel> UnvanlarListesi { get; set; } = new ObservableCollection<UnvanModel>();
        public static ObservableCollection<BirimModel> BirimlerListesi { get; set; } = new ObservableCollection<BirimModel>();
        public static ObservableCollection<YetkiModel> YetkilerListesi { get; set; } = new ObservableCollection<YetkiModel>();
        public static ObservableCollection<YetkiTipiModel> YetkiTipleriListesi { get; set; } = new ObservableCollection<YetkiTipiModel>();
        public static ObservableCollection<DosyaDetayModel> DosyaDetayListesi { get; set; } = new ObservableCollection<DosyaDetayModel>();
        public static ObservableCollection<BelgeDetayModel> BelgeDetayListesi { get; set; } = new ObservableCollection<BelgeDetayModel>();
        public static SistemAyarModel SistemAyarlar { get; set; } = new SistemAyarModel();
    }
}