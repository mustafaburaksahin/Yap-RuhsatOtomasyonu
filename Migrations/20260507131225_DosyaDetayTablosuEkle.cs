using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class DosyaDetayTablosuEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DosyaDetay",
                columns: table => new
                {
                    SiraNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YapiSiraNo = table.Column<int>(type: "int", nullable: false),
                    DosyaTipId = table.Column<int>(type: "int", nullable: true),
                    DosyaYolu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KayitTarihi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KaydedenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DegistirmeTarihi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DegistirenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DosyaDetay", x => x.SiraNo);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DosyaDetay");
        }
    }
}
