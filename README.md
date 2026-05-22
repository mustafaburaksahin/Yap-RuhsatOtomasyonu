# 🏢 Develi Belediyesi Yapı Ruhsat Otomasyonu

Develi Belediyesi için geliştirilmiş, yapı ruhsat süreçlerini dijitalleştiren modern, güvenli ve kullanıcı dostu bir masaüstü (WPF) otomasyon sistemidir. Eski sistemlere kıyasla veri bütünlüğünü koruyan, daha az sayfa ile daha çok işlem yapabilen modüler bir mimariye sahiptir.

## 🚀 Kullanılan Teknolojiler

* **Dil:** C#
* **UI Framework:** WPF (Windows Presentation Foundation)
* **Veritabanı & ORM:** SQL Server, Entity Framework Core (9.0.0)
* **Paketler / Kütüphaneler:**
  * `MaterialDesignThemes` (Modern UI/UX tasarımı)
  * `AWSSDK.S3` (Bulut depolama entegrasyonu)
  * `Microsoft.Web.WebView2`
  * `Microsoft.EntityFrameworkCore.SqlServer` & `Tools`

## ✨ Öne Çıkan Özellikler

### 🔐 Gelişmiş Yetkilendirme Sistemi (RBAC)
* **SuperAdmin, Admin ve User** olmak üzere 3 katmanlı rol yapısı.
* Kullanıcı yetkilerine göre sayfa, form, buton (Ekle/Sil/Güncelle) ve menülerin dinamik olarak gizlenmesi veya gösterilmesi.
* Güvenlik protokolleri gereği, Adminlerin birbirini veya SuperAdmin'i yönetmesini/görmesini engelleyen kısıtlamalar. Sadece SuperAdmin tüm hiyerarşiyi yönetebilir.

### 📊 İlişkisel Veri ve Arayüz Yönetimi
* Sicil (Müteahhit, Şantiye Şefi, Yapı Denetim Firması vb.), Yapı ve Belge verilerinin birbirine akıllı entegrasyonu.
* Veri tekrarını önlemek için çoklu sicil tiplerinin tek satırda birleştirilerek gösterilmesi.
* Yoğun veri tabloları için her sütuna özel **filtreleme** ve gelişmiş **arama çubukları**.
* Çoklu işlemleri kolaylaştıran **Google tarzı sekme (Tab)** mantığı.

### 📁 Akıllı Dosya Yönetimi
* Hedef cihazın IP adresine göre `Belgeler`, `Dosyalar` ve `Program Ayarları` olmak üzere otomatik yerel klasörleme sistemi.
* Depolama maliyetini ve bellek şişmesini önlemek için yüklenen tüm dosyaların **otomatik olarak ZIP formatında sıkıştırılması**.
* Yanlışlıkla veya yetkisiz dosya silinmesini engelleyen kısıtlanmış silme mekanizması.

### 🎨 Kurumsal Özelleştirme
* Program Ayarları ekranı üzerinden çalışma zamanında (runtime) uygulama logosu, il, ilçe ve belediye adı gibi kurumsal bilgilerin dinamik olarak güncellenmesi.

## ⚙️ Kurulum ve Çalıştırma

1. Projeyi bilgisayarınıza klonlayın:
   ```bash
   git clone [https://github.com/mustafaburaksahin/Yap-RuhsatOtomasyonu.git](https://github.com/mustafaburaksahin/Yap-RuhsatOtomasyonu.git)
