using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class ModelkısmınaDegistirenkullanıcıEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Yetkiler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Yapilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Unvanlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Siciller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Dosyalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Birimler",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Yetkiler");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Yapilar");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Unvanlar");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Siciller");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Dosyalar");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Birimler");
        }
    }
}
