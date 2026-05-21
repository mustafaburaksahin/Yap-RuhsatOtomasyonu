using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class DegistirmeTarihiEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "Yetkiler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "Yapilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "Unvanlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "SicilTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "Siciller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "RuhsatAmaclari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "RuhsatAmaclari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "IskanAmaclari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "IskanAmaclari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "DosyaTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "DosyaTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenTarihi",
                table: "Dosyalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "Birimler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "BelgeTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "BelgeTipleri",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "Yetkiler");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "Yapilar");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "Unvanlar");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "SicilTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "Siciller");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "RuhsatAmaclari");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "RuhsatAmaclari");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "IskanAmaclari");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "IskanAmaclari");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "DosyaTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "DosyaTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirenTarihi",
                table: "Dosyalar");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "Birimler");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "BelgeTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "BelgeTipleri");
        }
    }
}
