using Microsoft.EntityFrameworkCore.Migrations;

namespace Dissimilis.DbContext.Migrations
{
    public partial class IndexUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SongBars_SongVoiceId_Position",
                table: "SongBars");

            migrationBuilder.CreateIndex(
                name: "IX_SongBars_SongVoiceId_Position",
                table: "SongBars",
                columns: new[] { "SongVoiceId", "Position" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SongBars_SongVoiceId_Position",
                table: "SongBars");

            migrationBuilder.CreateIndex(
                name: "IX_SongBars_SongVoiceId_Position",
                table: "SongBars",
                columns: new[] { "SongVoiceId", "Position" },
                unique: true);
        }
    }
}
