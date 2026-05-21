using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class BelgeDetayTablosuEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BelgeDetaylar",
                columns: table => new
                {
                    SiraNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YapiSiraNo = table.Column<int>(type: "int", nullable: false),
                    RuhsatNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YapiKimlikNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RuhsatBilgileri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RuhsatVerilisAmaci = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RuhsatTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParselinAlani = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsaatHarci = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YapiDenetimFirmasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SantiyeSefi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Muteahhit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IskanVerilisAmaci = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IskanTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BelgeyeEsasRuhsat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IskanNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BelgeAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BelgeYolu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaydedenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirmeTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BelgeDetaylar", x => x.SiraNo);
                    table.ForeignKey(
                        name: "FK_BelgeDetaylar_Yapilar_YapiSiraNo",
                        column: x => x.YapiSiraNo,
                        principalTable: "Yapilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BelgeDetaylar_YapiSiraNo",
                table: "BelgeDetaylar",
                column: "YapiSiraNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BelgeDetaylar");
        }
    }
}
