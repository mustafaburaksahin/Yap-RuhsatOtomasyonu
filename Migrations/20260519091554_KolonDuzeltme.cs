using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class KolonDuzeltme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "BelgeDetaylar");

            migrationBuilder.DropColumn(
                name: "DegistirmeTarihi",
                table: "BelgeDetaylar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
