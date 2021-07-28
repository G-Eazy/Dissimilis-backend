using Microsoft.EntityFrameworkCore.Migrations;

namespace Dissimilis.DbContext.Migrations
{
    public partial class VoltaBracketRenameAndAddCascademigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupUsers_Groups_GroupId",
                table: "GroupUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupUsers_Users_UserId",
                table: "GroupUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganisationUsers_Organisations_OrganisationId",
                table: "OrganisationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganisationUsers_Users_UserId",
                table: "OrganisationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_SongGroupTags_Groups_GroupId",
                table: "SongGroupTags");

            migrationBuilder.DropForeignKey(
                name: "FK_SongGroupTags_Songs_SongId",
                table: "SongGroupTags");

            migrationBuilder.DropForeignKey(
                name: "FK_SongOrganisationTags_Organisations_OrganisationId",
                table: "SongOrganisationTags");

            migrationBuilder.DropForeignKey(
                name: "FK_SongOrganisationTags_Songs_SongId",
                table: "SongOrganisationTags");

            migrationBuilder.DropForeignKey(
                name: "FK_SongSharedUser_Songs_SongId",
                table: "SongSharedUser");

            migrationBuilder.DropForeignKey(
                name: "FK_SongSharedUser_Users_UserId",
                table: "SongSharedUser");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUsers_Groups_GroupId",
                table: "GroupUsers",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUsers_Users_UserId",
                table: "GroupUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganisationUsers_Organisations_OrganisationId",
                table: "OrganisationUsers",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganisationUsers_Users_UserId",
                table: "OrganisationUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongGroupTags_Groups_GroupId",
                table: "SongGroupTags",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongGroupTags_Songs_SongId",
                table: "SongGroupTags",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongOrganisationTags_Organisations_OrganisationId",
                table: "SongOrganisationTags",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongOrganisationTags_Songs_SongId",
                table: "SongOrganisationTags",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongSharedUser_Songs_SongId",
                table: "SongSharedUser",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongSharedUser_Users_UserId",
                table: "SongSharedUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupUsers_Groups_GroupId",
                table: "GroupUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupUsers_Users_UserId",
                table: "GroupUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganisationUsers_Organisations_OrganisationId",
                table: "OrganisationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganisationUsers_Users_UserId",
                table: "OrganisationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_SongGroupTags_Groups_GroupId",
                table: "SongGroupTags");

            migrationBuilder.DropForeignKey(
                name: "FK_SongGroupTags_Songs_SongId",
                table: "SongGroupTags");

            migrationBuilder.DropForeignKey(
                name: "FK_SongOrganisationTags_Organisations_OrganisationId",
                table: "SongOrganisationTags");

            migrationBuilder.DropForeignKey(
                name: "FK_SongOrganisationTags_Songs_SongId",
                table: "SongOrganisationTags");

            migrationBuilder.DropForeignKey(
                name: "FK_SongSharedUser_Songs_SongId",
                table: "SongSharedUser");

            migrationBuilder.DropForeignKey(
                name: "FK_SongSharedUser_Users_UserId",
                table: "SongSharedUser");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUsers_Groups_GroupId",
                table: "GroupUsers",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUsers_Users_UserId",
                table: "GroupUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganisationUsers_Organisations_OrganisationId",
                table: "OrganisationUsers",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganisationUsers_Users_UserId",
                table: "OrganisationUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SongGroupTags_Groups_GroupId",
                table: "SongGroupTags",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SongGroupTags_Songs_SongId",
                table: "SongGroupTags",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SongOrganisationTags_Organisations_OrganisationId",
                table: "SongOrganisationTags",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SongOrganisationTags_Songs_SongId",
                table: "SongOrganisationTags",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SongSharedUser_Songs_SongId",
                table: "SongSharedUser",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SongSharedUser_Users_UserId",
                table: "SongSharedUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
