using Microsoft.EntityFrameworkCore.Migrations;

namespace Dissimilis.DbContext.Migrations
{
    public partial class EditSongMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DegreeOfDifficulty",
                table: "Songs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SongNotes",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Speed",
                table: "Songs",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegreeOfDifficulty",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "SongNotes",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "Speed",
                table: "Songs");
        }
    }
}
