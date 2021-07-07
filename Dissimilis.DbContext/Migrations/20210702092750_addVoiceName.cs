using Microsoft.EntityFrameworkCore.Migrations;

namespace Dissimilis.DbContext.Migrations
{
    public partial class addVoiceName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VoiceName",
                table: "SongVoices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoiceName",
                table: "SongVoices");
        }
    }
}
