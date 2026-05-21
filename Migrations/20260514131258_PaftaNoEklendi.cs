using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YapıRuhsatOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class PaftaNoEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegistirenKullanici",
                table: "Yapilar");

            migrationBuilder.RenameColumn(
                name: "DegistirmeTarihi",
                table: "Yapilar",
                newName: "PaftaNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaftaNo",
                table: "Yapilar",
                newName: "DegistirmeTarihi");

            migrationBuilder.AddColumn<string>(
                name: "DegistirenKullanici",
                table: "Yapilar",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
