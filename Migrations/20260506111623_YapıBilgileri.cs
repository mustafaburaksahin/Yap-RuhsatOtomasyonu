using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class YapıBilgileri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "KaydedenKullanici",
                table: "RuhsatAmaclari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KayitTarihi",
                table: "RuhsatAmaclari",
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
                name: "KaydedenKullanici",
                table: "IskanAmaclari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KayitTarihi",
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
                name: "KaydedenKullanici",
                table: "DosyaTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KayitTarihi",
                table: "DosyaTipleri",
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

            migrationBuilder.AddColumn<string>(
                name: "KaydedenKullanici",
                table: "BelgeTipleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KayitTarihi",
                table: "BelgeTipleri",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "RuhsatAmaclari");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "RuhsatAmaclari");

            migrationBuilder.DropColumn(
                name: "KaydedenKullanici",
                table: "RuhsatAmaclari");

            migrationBuilder.DropColumn(
                name: "KayitTarihi",
                table: "RuhsatAmaclari");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "IskanAmaclari");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "IskanAmaclari");

            migrationBuilder.DropColumn(
                name: "KaydedenKullanici",
                table: "IskanAmaclari");

            migrationBuilder.DropColumn(
                name: "KayitTarihi",
                table: "IskanAmaclari");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "DosyaTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "DosyaTipleri");

            migrationBuilder.DropColumn(
                name: "KaydedenKullanici",
                table: "DosyaTipleri");

            migrationBuilder.DropColumn(
                name: "KayitTarihi",
                table: "DosyaTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "BelgeTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "BelgeTipleri");

            migrationBuilder.DropColumn(
                name: "KaydedenKullanici",
                table: "BelgeTipleri");

            migrationBuilder.DropColumn(
                name: "KayitTarihi",
                table: "BelgeTipleri");
        }
    }
}
