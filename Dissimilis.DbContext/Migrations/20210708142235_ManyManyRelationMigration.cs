using Microsoft.EntityFrameworkCore.Migrations;

namespace Dissimilis.DbContext.Migrations
{
    public partial class ManyManyRelationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Songs_SongId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisations_Songs_SongId",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_SongId",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Groups_SongId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "SongId",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "SongId",
                table: "Groups");

            migrationBuilder.CreateTable(
                name: "SongSharedGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongSharedGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SongSharedGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SongSharedGroups_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SongSharedOrganisations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongSharedOrganisations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SongSharedOrganisations_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SongSharedOrganisations_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SongSharedGroups_GroupId",
                table: "SongSharedGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SongSharedGroups_SongId",
                table: "SongSharedGroups",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_SongSharedOrganisations_OrganisationId",
                table: "SongSharedOrganisations",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_SongSharedOrganisations_SongId",
                table: "SongSharedOrganisations",
                column: "SongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongSharedGroups");

            migrationBuilder.DropTable(
                name: "SongSharedOrganisations");

            migrationBuilder.AddColumn<int>(
                name: "SongId",
                table: "Organisations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SongId",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_SongId",
                table: "Organisations",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_SongId",
                table: "Groups",
                column: "SongId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Songs_SongId",
                table: "Groups",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organisations_Songs_SongId",
                table: "Organisations",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
