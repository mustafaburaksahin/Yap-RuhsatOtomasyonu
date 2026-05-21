using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class YetkiSistemiEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Yetkiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YetkiTipi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modul = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Panel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YetkiVerildiMi = table.Column<bool>(type: "bit", nullable: false),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaydedenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirmeTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yetkiler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YetkiTipleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YetkiTipiAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaydedenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirmeTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YetkiTipleri", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Yetkiler");

            migrationBuilder.DropTable(
                name: "YetkiTipleri");
        }
    }
}
