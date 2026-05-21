using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class SicilBilgileri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "SicilTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "SicilTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KaydedenKullanici",
                table: "SicilTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KayitTarihi",
                table: "SicilTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Siciller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "Siciller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KaydedenKullanici",
                table: "Siciller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KayitTarihi",
                table: "Siciller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "KullaniciTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "KullaniciTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KaydedenKullanici",
                table: "KullaniciTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KayitTarihi",
                table: "KullaniciTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KaydedenKullanici",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KayitTarihi",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "SicilTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "SicilTipleri");

            migrationBuilder.DropColumn(
                name: "KaydedenKullanici",
                table: "SicilTipleri");

            migrationBuilder.DropColumn(
                name: "KayitTarihi",
                table: "SicilTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Siciller");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "Siciller");

            migrationBuilder.DropColumn(
                name: "KaydedenKullanici",
                table: "Siciller");

            migrationBuilder.DropColumn(
                name: "KayitTarihi",
                table: "Siciller");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "KullaniciTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "KullaniciTipleri");

            migrationBuilder.DropColumn(
                name: "KaydedenKullanici",
                table: "KullaniciTipleri");

            migrationBuilder.DropColumn(
                name: "KayitTarihi",
                table: "KullaniciTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "KaydedenKullanici",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "KayitTarihi",
                table: "Kullanicilar");
        }
    }
}
