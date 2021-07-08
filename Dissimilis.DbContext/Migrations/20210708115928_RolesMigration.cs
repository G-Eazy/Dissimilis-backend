using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dissimilis.DbContext.Migrations
{
    public partial class RolesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_UpdatedById",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisations_Users_UpdatedById",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_UpdatedById",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Groups_UpdatedById",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "MsId",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "GroupInfo",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "isSystemAdmin",
                table: "Users",
                newName: "IsSystemAdmin");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "OrganisationUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrganisationId",
                table: "OrganisationUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "GroupUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "GroupUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrganisationId",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Groups",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSystemAdmin",
                table: "Users",
                newName: "isSystemAdmin");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "OrganisationUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OrganisationId",
                table: "OrganisationUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "MsId",
                table: "Organisations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "Organisations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "Organisations",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "GroupUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "GroupUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OrganisationId",
                table: "Groups",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "GroupInfo",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "Groups",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_UpdatedById",
                table: "Organisations",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_UpdatedById",
                table: "Groups",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_UpdatedById",
                table: "Groups",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organisations_Users_UpdatedById",
                table: "Organisations",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
