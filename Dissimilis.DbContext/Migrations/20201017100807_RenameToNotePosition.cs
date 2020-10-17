using Microsoft.EntityFrameworkCore.Migrations;

namespace Dissimilis.DbContext.Migrations
{
    public partial class RenameToNotePosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SongNotes_BarId_NoteNumber",
                table: "SongNotes");

            migrationBuilder.DropIndex(
                name: "IX_SongBars_SongVoiceId_BarNumber",
                table: "SongBars");

            migrationBuilder.DropColumn(
                name: "NoteNumber",
                table: "SongNotes");

            migrationBuilder.DropColumn(
                name: "BarNumber",
                table: "SongBars");

            migrationBuilder.AddColumn<int>(
                name: "Postition",
                table: "SongNotes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "SongBars",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SongNotes_BarId_Postition",
                table: "SongNotes",
                columns: new[] { "BarId", "Postition" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SongBars_SongVoiceId_Position",
                table: "SongBars",
                columns: new[] { "SongVoiceId", "Position" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SongNotes_BarId_Postition",
                table: "SongNotes");

            migrationBuilder.DropIndex(
                name: "IX_SongBars_SongVoiceId_Position",
                table: "SongBars");

            migrationBuilder.DropColumn(
                name: "Postition",
                table: "SongNotes");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "SongBars");

            migrationBuilder.AddColumn<int>(
                name: "NoteNumber",
                table: "SongNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BarNumber",
                table: "SongBars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SongNotes_BarId_NoteNumber",
                table: "SongNotes",
                columns: new[] { "BarId", "NoteNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SongBars_SongVoiceId_BarNumber",
                table: "SongBars",
                columns: new[] { "SongVoiceId", "BarNumber" },
                unique: true);
        }
    }
}
