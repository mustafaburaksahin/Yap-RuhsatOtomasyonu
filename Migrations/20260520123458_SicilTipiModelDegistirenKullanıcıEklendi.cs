using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class SicilTipiModelDegistirenKullanıcıEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "SicilTipleri",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "SicilTipleri");
        }
    }
}
