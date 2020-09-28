using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dissimilis.DbContext.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MsId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MsId = table.Column<string>(maxLength: 200, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 150, nullable: false),
                    CountryId = table.Column<int>(nullable: true),
                    OrganisationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 500, nullable: false),
                    Composer = table.Column<string>(maxLength: 100, nullable: true),
                    Numerator = table.Column<int>(nullable: false),
                    Denominator = table.Column<int>(nullable: false),
                    ArrangerId = table.Column<int>(nullable: false),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    UpdatedById = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Songs_Users_ArrangerId",
                        column: x => x.ArrangerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Songs_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Songs_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SongVoices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoiceNumber = table.Column<int>(nullable: false),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    UpdatedById = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: true),
                    InstrumentId = table.Column<int>(nullable: false),
                    SongId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongVoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SongVoices_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SongVoices_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SongVoices_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongVoices_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SongBars",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarNumber = table.Column<int>(nullable: false),
                    RepBefore = table.Column<bool>(nullable: false),
                    RepAfter = table.Column<bool>(nullable: false),
                    House = table.Column<int>(nullable: true),
                    SongVoiceId = table.Column<int>(nullable: false),
                    SongVoiceId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongBars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SongBars_SongVoices_SongVoiceId",
                        column: x => x.SongVoiceId,
                        principalTable: "SongVoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongBars_SongVoices_SongVoiceId1",
                        column: x => x.SongVoiceId1,
                        principalTable: "SongVoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SongNotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoteNumber = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    NoteValues = table.Column<string>(maxLength: 100, nullable: true),
                    BarId = table.Column<int>(nullable: false),
                    SongBarId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SongNotes_SongBars_BarId",
                        column: x => x.BarId,
                        principalTable: "SongBars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongNotes_SongBars_SongBarId1",
                        column: x => x.SongBarId1,
                        principalTable: "SongBars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_Name",
                table: "Instruments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_Name",
                table: "Organisations",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SongBars_SongVoiceId1",
                table: "SongBars",
                column: "SongVoiceId1");

            migrationBuilder.CreateIndex(
                name: "IX_SongBars_SongVoiceId_BarNumber",
                table: "SongBars",
                columns: new[] { "SongVoiceId", "BarNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SongNotes_SongBarId1",
                table: "SongNotes",
                column: "SongBarId1");

            migrationBuilder.CreateIndex(
                name: "IX_SongNotes_BarId_NoteNumber",
                table: "SongNotes",
                columns: new[] { "BarId", "NoteNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Songs_ArrangerId",
                table: "Songs",
                column: "ArrangerId");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_CreatedById",
                table: "Songs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_UpdatedById",
                table: "Songs",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SongVoices_CreatedById",
                table: "SongVoices",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SongVoices_InstrumentId",
                table: "SongVoices",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_SongVoices_UpdatedById",
                table: "SongVoices",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SongVoices_SongId_VoiceNumber",
                table: "SongVoices",
                columns: new[] { "SongId", "VoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CountryId",
                table: "Users",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_MsId",
                table: "Users",
                column: "MsId",
                unique: true,
                filter: "[MsId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OrganisationId",
                table: "Users",
                column: "OrganisationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongNotes");

            migrationBuilder.DropTable(
                name: "SongBars");

            migrationBuilder.DropTable(
                name: "SongVoices");

            migrationBuilder.DropTable(
                name: "Instruments");

            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Organisations");
        }
    }
}
