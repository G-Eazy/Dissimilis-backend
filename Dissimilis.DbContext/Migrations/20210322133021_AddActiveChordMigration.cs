using Microsoft.EntityFrameworkCore.Migrations;

namespace Dissimilis.DbContext.Migrations
{
    public partial class AddActiveChordMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Postition",
                table: "SongNotes",
                newName: "Position");

            migrationBuilder.RenameIndex(
                name: "IX_SongNotes_BarId_Postition",
                table: "SongNotes",
                newName: "IX_SongNotes_BarId_Position");

            migrationBuilder.AddColumn<string>(
                name: "ActiveChord",
                table: "SongNotes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveChord",
                table: "SongNotes");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "SongNotes",
                newName: "Postition");

            migrationBuilder.RenameIndex(
                name: "IX_SongNotes_BarId_Position",
                table: "SongNotes",
                newName: "IX_SongNotes_BarId_Postition");
        }
    }
}
