using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class SonGuncelleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Moduller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModulAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaydedenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirmeTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    PanelAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModulAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigerBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KaydedenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegistirmeTarihi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paneller", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Moduller");

            migrationBuilder.DropTable(
                name: "Paneller");
        }
    }
}
