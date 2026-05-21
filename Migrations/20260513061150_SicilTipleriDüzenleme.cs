using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class SicilTipleriDüzenleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KullaniciTipleri");

            migrationBuilder.DropColumn(
                name: "BilirKisi",
                table: "Siciller");

            migrationBuilder.AddColumn<int>(
                name: "SicilModelId",
                table: "SicilTipleri",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SicilTipleri_SicilModelId",
                table: "SicilTipleri",
                column: "SicilModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_SicilTipleri_Siciller_SicilModelId",
                table: "SicilTipleri",
                column: "SicilModelId",
                principalTable: "Siciller",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SicilTipleri_Siciller_SicilModelId",
                table: "SicilTipleri");

            migrationBuilder.DropIndex(
                name: "IX_SicilTipleri_SicilModelId",
                table: "SicilTipleri");

            migrationBuilder.DropColumn(
                name: "SicilModelId",
                table: "SicilTipleri");

            migrationBuilder.AddColumn<string>(
                name: "BilirKisi",
                table: "Siciller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KullaniciTipleri",
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
                    KullaniciTipi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciTipleri", x => x.Id);
                });
        }
    }
}
