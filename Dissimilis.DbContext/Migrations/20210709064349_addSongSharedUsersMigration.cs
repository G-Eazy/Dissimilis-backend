using Microsoft.EntityFrameworkCore.Migrations;

namespace Dissimilis.DbContext.Migrations
{
    public partial class addSongSharedUsersMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Organisations_OrganisationId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Songs_SongId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SongId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SongId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "SongSharedUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongSharedUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SongSharedUser_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SongSharedUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SongSharedUser_SongId",
                table: "SongSharedUser",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_SongSharedUser_UserId",
                table: "SongSharedUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Organisations_OrganisationId",
                table: "Groups",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Organisations_OrganisationId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "SongSharedUser");

            migrationBuilder.AddColumn<int>(
                name: "SongId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SongId",
                table: "Users",
                column: "SongId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Organisations_OrganisationId",
                table: "Groups",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Songs_SongId",
                table: "Users",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
