using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class AddYetkiColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Moduller");

            migrationBuilder.DropTable(
                name: "Paneller");

            migrationBuilder.DropTable(
                name: "YetkiTipleri");

            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "Yetkiler");

            migrationBuilder.DropColumn(
                name: "DigerBilgiler",
                table: "Yetkiler");

            migrationBuilder.DropColumn(
                name: "YetkiTipi",
                table: "Yetkiler");

            migrationBuilder.AddColumn<bool>(
                name: "EkleYetki",
                table: "Yetkiler",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GuncelleYetki",
                table: "Yetkiler",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SilYetki",
                table: "Yetkiler",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EkleYetki",
                table: "Yetkiler");

            migrationBuilder.DropColumn(
                name: "GuncelleYetki",
                table: "Yetkiler");

            migrationBuilder.DropColumn(
                name: "SilYetki",
                table: "Yetkiler");

            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "Yetkiler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DigerBilgiler",
                table: "Yetkiler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YetkiTipi",
                table: "Yetkiler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Moduller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirmeTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaydedenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModulAdi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moduller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Paneller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirmeTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaydedenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModulAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PanelAdi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paneller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YetkiTipleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirmeTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaydedenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YetkiTipiAdi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YetkiTipleri", x => x.Id);
                });
        }
    }
}
