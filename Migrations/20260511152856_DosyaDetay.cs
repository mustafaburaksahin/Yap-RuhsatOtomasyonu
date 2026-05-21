using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class DosyaDetay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DosyaDetay_DosyaTipId",
                table: "DosyaDetay",
                column: "DosyaTipId");

            migrationBuilder.AddForeignKey(
                name: "FK_DosyaDetay_DosyaTipleri_DosyaTipId",
                table: "DosyaDetay",
                column: "DosyaTipId",
                principalTable: "DosyaTipleri",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DosyaDetay_DosyaTipleri_DosyaTipId",
                table: "DosyaDetay");

            migrationBuilder.DropIndex(
                name: "IX_DosyaDetay_DosyaTipId",
                table: "DosyaDetay");
        }
    }
}
