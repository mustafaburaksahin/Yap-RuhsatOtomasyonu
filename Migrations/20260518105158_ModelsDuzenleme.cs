using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class ModelsDuzenleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Yetkiler");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "Yetkiler");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Unvanlar");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "Unvanlar");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "SicilTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "SicilTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Siciller");

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
                name: "Birim",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "KullaniciBirimi",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "Unvan",
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
                name: "DegistirenKullanici",
                table: "DosyaDetay");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "DosyaDetay");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Birimler");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "Birimler");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "BelgeTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "BelgeTipleri");

            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "BelgeDetaylar");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "BelgeDetaylar");

            migrationBuilder.AddColumn<int>(
                name: "BirimId",
                table: "Kullanicilar",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnvanId",
                table: "Kullanicilar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BelgeModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BelgeAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BelgeYolu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uzanti = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaydedenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YapiSiraNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BelgeModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BelgeModel_Yapilar_YapiSiraNo",
                        column: x => x.YapiSiraNo,
                        principalTable: "Yapilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_BirimId",
                table: "Kullanicilar",
                column: "BirimId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_UnvanId",
                table: "Kullanicilar",
                column: "UnvanId");

            migrationBuilder.CreateIndex(
                name: "IX_Dosyalar_YapiSiraNo",
                table: "Dosyalar",
                column: "YapiSiraNo");

            migrationBuilder.CreateIndex(
                name: "IX_DosyaDetay_YapiSiraNo",
                table: "DosyaDetay",
                column: "YapiSiraNo");

            migrationBuilder.CreateIndex(
                name: "IX_BelgeModel_YapiSiraNo",
                table: "BelgeModel",
                column: "YapiSiraNo");

            migrationBuilder.AddForeignKey(
                name: "FK_DosyaDetay_Yapilar_YapiSiraNo",
                table: "DosyaDetay",
                column: "YapiSiraNo",
                principalTable: "Yapilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dosyalar_Yapilar_YapiSiraNo",
                table: "Dosyalar",
                column: "YapiSiraNo",
                principalTable: "Yapilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Birimler_BirimId",
                table: "Kullanicilar",
                column: "BirimId",
                principalTable: "Birimler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Unvanlar_UnvanId",
                table: "Kullanicilar",
                column: "UnvanId",
                principalTable: "Unvanlar",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DosyaDetay_Yapilar_YapiSiraNo",
                table: "DosyaDetay");

            migrationBuilder.DropForeignKey(
                name: "FK_Dosyalar_Yapilar_YapiSiraNo",
                table: "Dosyalar");

            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_Birimler_BirimId",
                table: "Kullanicilar");

            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_Unvanlar_UnvanId",
                table: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "BelgeModel");

            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_BirimId",
                table: "Kullanicilar");

            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_UnvanId",
                table: "Kullanicilar");

            migrationBuilder.DropIndex(
                name: "IX_Dosyalar_YapiSiraNo",
                table: "Dosyalar");

            migrationBuilder.DropIndex(
                name: "IX_DosyaDetay_YapiSiraNo",
                table: "DosyaDetay");

            migrationBuilder.DropColumn(
                name: "BirimId",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "UnvanId",
                table: "Kullanicilar");

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Yetkiler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "Yetkiler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Unvanlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "Unvanlar",
                type: "nvarchar(max)",
                nullable: true);

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
                name: "Birim",
                table: "Kullanicilar",
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
                name: "KullaniciBirimi",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unvan",
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
                name: "DegistirenKullanici",
                table: "DosyaDetay",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "DosyaDetay",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Birimler",
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

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "BelgeDetaylar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegistirmeTarihi",
                table: "BelgeDetaylar",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
