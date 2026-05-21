using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class BelgeEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BelgeTipleri",
                table: "BelgeTipleri");

            migrationBuilder.RenameTable(
                name: "BelgeTipleri",
                newName: "BelgeTipiModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BelgeTipiModel",
                table: "BelgeTipiModel",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BelgeTipiModel",
                table: "BelgeTipiModel");

            migrationBuilder.RenameTable(
                name: "BelgeTipiModel",
                newName: "BelgeTipleri");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BelgeTipleri",
                table: "BelgeTipleri",
                column: "Id");
        }
    }
}
