using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class SistemAyarlariEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SistemAyarlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SistemAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BelediyeAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Il = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ilce = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoYolu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SistemAyarlari", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SistemAyarlari");
        }
    }
}
