using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class ModelIskonveYetkiGuncelleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DosyaDetay_Yapilar_YapiSiraNo",
                table: "DosyaDetay");

            migrationBuilder.DropForeignKey(
                name: "FK_Dosyalar_Yapilar_YapiSiraNo",
                table: "Dosyalar");

            migrationBuilder.DropForeignKey(
                name: "FK_SicilTipleri_Siciller_SicilModelId",
                table: "SicilTipleri");

            migrationBuilder.DropTable(
                name: "BelgeModel");

            migrationBuilder.DropIndex(
                name: "IX_SicilTipleri_SicilModelId",
                table: "SicilTipleri");

            migrationBuilder.DropIndex(
                name: "IX_Dosyalar_YapiSiraNo",
                table: "Dosyalar");

            migrationBuilder.DropIndex(
                name: "IX_DosyaDetay_YapiSiraNo",
                table: "DosyaDetay");

            migrationBuilder.DropColumn(
                name: "SicilModelId",
                table: "SicilTipleri");

            migrationBuilder.DropColumn(
                name: "BilmemNo",
                table: "Siciller");

            migrationBuilder.RenameColumn(
                name: "RuhsatVerilisAmaci",
                table: "BelgeDetaylar",
                newName: "DegistirmeTarihi");

            migrationBuilder.RenameColumn(
                name: "IskanVerilisAmaci",
                table: "BelgeDetaylar",
                newName: "DegistirenKullanici");

            migrationBuilder.AlterColumn<int>(
                name: "SicilTipi",
                table: "Siciller",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KullaniciBirimi",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YetkiId",
                table: "Kullanicilar",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IskanVerilisAmaciId",
                table: "BelgeDetaylar",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RuhsatVerilisAmaciId",
                table: "BelgeDetaylar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Siciller_SicilTipi",
                table: "Siciller",
                column: "SicilTipi");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_YetkiId",
                table: "Kullanicilar",
                column: "YetkiId");

            migrationBuilder.CreateIndex(
                name: "IX_BelgeDetaylar_IskanVerilisAmaciId",
                table: "BelgeDetaylar",
                column: "IskanVerilisAmaciId");

            migrationBuilder.CreateIndex(
                name: "IX_BelgeDetaylar_RuhsatVerilisAmaciId",
                table: "BelgeDetaylar",
                column: "RuhsatVerilisAmaciId");

            migrationBuilder.AddForeignKey(
                name: "FK_BelgeDetaylar_IskanAmaclari_IskanVerilisAmaciId",
                table: "BelgeDetaylar",
                column: "IskanVerilisAmaciId",
                principalTable: "IskanAmaclari",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BelgeDetaylar_RuhsatAmaclari_RuhsatVerilisAmaciId",
                table: "BelgeDetaylar",
                column: "RuhsatVerilisAmaciId",
                principalTable: "RuhsatAmaclari",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Yetkiler_YetkiId",
                table: "Kullanicilar",
                column: "YetkiId",
                principalTable: "Yetkiler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Siciller_SicilTipleri_SicilTipi",
                table: "Siciller",
                column: "SicilTipi",
                principalTable: "SicilTipleri",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BelgeDetaylar_IskanAmaclari_IskanVerilisAmaciId",
                table: "BelgeDetaylar");

            migrationBuilder.DropForeignKey(
                name: "FK_BelgeDetaylar_RuhsatAmaclari_RuhsatVerilisAmaciId",
                table: "BelgeDetaylar");

            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_Yetkiler_YetkiId",
                table: "Kullanicilar");

            migrationBuilder.DropForeignKey(
                name: "FK_Siciller_SicilTipleri_SicilTipi",
                table: "Siciller");

            migrationBuilder.DropIndex(
                name: "IX_Siciller_SicilTipi",
                table: "Siciller");

            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_YetkiId",
                table: "Kullanicilar");

            migrationBuilder.DropIndex(
                name: "IX_BelgeDetaylar_IskanVerilisAmaciId",
                table: "BelgeDetaylar");

            migrationBuilder.DropIndex(
                name: "IX_BelgeDetaylar_RuhsatVerilisAmaciId",
                table: "BelgeDetaylar");

            migrationBuilder.DropColumn(
                name: "KullaniciBirimi",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "YetkiId",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "IskanVerilisAmaciId",
                table: "BelgeDetaylar");

            migrationBuilder.DropColumn(
                name: "RuhsatVerilisAmaciId",
                table: "BelgeDetaylar");

            migrationBuilder.RenameColumn(
                name: "DegistirmeTarihi",
                table: "BelgeDetaylar",
                newName: "RuhsatVerilisAmaci");

            migrationBuilder.RenameColumn(
                name: "DegistirenKullanici",
                table: "BelgeDetaylar",
                newName: "IskanVerilisAmaci");

            migrationBuilder.AddColumn<int>(
                name: "SicilModelId",
                table: "SicilTipleri",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SicilTipi",
                table: "Siciller",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BilmemNo",
                table: "Siciller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BelgeModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YapiSiraNo = table.Column<int>(type: "int", nullable: false),
                    BelgeAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BelgeYolu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaydedenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uzanti = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "IX_SicilTipleri_SicilModelId",
                table: "SicilTipleri",
                column: "SicilModelId");

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
                name: "FK_SicilTipleri_Siciller_SicilModelId",
                table: "SicilTipleri",
                column: "SicilModelId",
                principalTable: "Siciller",
                principalColumn: "Id");
        }
    }
}
