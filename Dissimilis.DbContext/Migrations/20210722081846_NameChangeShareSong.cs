using Microsoft.EntityFrameworkCore.Migrations;

namespace Dissimilis.DbContext.Migrations
{
    public partial class NameChangeShareSong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongSharedGroups");

            migrationBuilder.DropTable(
                name: "SongSharedOrganisations");

            migrationBuilder.CreateTable(
                name: "SongGroupTags",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongGroupTags", x => new { x.GroupId, x.SongId });
                    table.ForeignKey(
                        name: "FK_SongGroupTags_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SongGroupTags_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SongOrganisationTags",
                columns: table => new
                {
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongOrganisationTags", x => new { x.OrganisationId, x.SongId });
                    table.ForeignKey(
                        name: "FK_SongOrganisationTags_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SongOrganisationTags_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SongGroupTags_SongId",
                table: "SongGroupTags",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_SongOrganisationTags_SongId",
                table: "SongOrganisationTags",
                column: "SongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongGroupTags");

            migrationBuilder.DropTable(
                name: "SongOrganisationTags");

            migrationBuilder.CreateTable(
                name: "SongSharedGroups",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongSharedGroups", x => new { x.GroupId, x.SongId });
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
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongSharedOrganisations", x => new { x.OrganisationId, x.SongId });
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
                name: "IX_SongSharedGroups_SongId",
                table: "SongSharedGroups",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_SongSharedOrganisations_SongId",
                table: "SongSharedOrganisations",
                column: "SongId");
        }
    }
}
