using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class IlkKurulum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BelgeTipleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BelgeTipleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DosyaTipleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DosyaTipleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IskanAmaclari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VerilisAmaci = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IskanAmaclari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DogumYeri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DogumTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnneAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BabaAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TelefonNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GsmNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sifre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KullaniciTipi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unvan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Birim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tanim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KullaniciBirimi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciTipleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciTipi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciTipleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RuhsatAmaclari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VerilisAmaci = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuhsatAmaclari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Siciller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SicilTipi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VergiNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirmaAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DogumYeri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DogumTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VergiDairesi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BilmemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnneAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BabaAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TelefonNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BilirKisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlanKisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siciller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SicilTipleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SicilTipi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SicilTipleri", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BelgeTipleri");

            migrationBuilder.DropTable(
                name: "DosyaTipleri");

            migrationBuilder.DropTable(
                name: "IskanAmaclari");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "KullaniciTipleri");

            migrationBuilder.DropTable(
                name: "RuhsatAmaclari");

            migrationBuilder.DropTable(
                name: "Siciller");

            migrationBuilder.DropTable(
                name: "SicilTipleri");
        }
    }
}
