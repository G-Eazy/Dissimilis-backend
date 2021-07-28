﻿// <auto-generated />
using System;
using Dissimilis.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dissimilis.DbContext.Migrations
{
    [DbContext(typeof(DissimilisDbContext))]
    [Migration("20210709080406_GroupOrgSongMigration")]
    partial class GroupOrgSongMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Dissimilis.DbContext.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("CreatedById")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset?>("CreatedOn")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("OrganisationId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.GroupUser", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("UserId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("GroupUsers");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Instrument", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("DefinedInstrument")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Instruments");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Organisation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("CreatedById")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset?>("CreatedOn")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.OrganisationUser", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("UserId", "OrganisationId");

                    b.HasIndex("OrganisationId");

                    b.ToTable("OrganisationUsers");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Song.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("ArrangerId")
                        .HasColumnType("int");

                    b.Property<string>("Composer")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("CreatedById")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset?>("CreatedOn")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("DegreeOfDifficulty")
                        .HasColumnType("int");

                    b.Property<int>("Denominator")
                        .HasColumnType("int");

                    b.Property<int>("Numerator")
                        .HasColumnType("int");

                    b.Property<int>("ProtectionLevel")
                        .HasColumnType("int");

                    b.Property<string>("SongNotes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Speed")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int?>("UpdatedById")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset?>("UpdatedOn")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("ArrangerId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdatedById");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Song.SongBar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("VoltaBracket")
                        .HasColumnType("int");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<bool>("RepAfter")
                        .HasColumnType("bit");

                    b.Property<bool>("RepBefore")
                        .HasColumnType("bit");

                    b.Property<int>("SongVoiceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SongVoiceId", "Position");

                    b.ToTable("SongBars");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Song.SongNote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("BarId")
                        .HasColumnType("int");

                    b.Property<string>("ChordName")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("Length")
                        .HasColumnType("int");

                    b.Property<string>("NoteValues")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BarId", "Position")
                        .IsUnique();

                    b.ToTable("SongNotes");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Song.SongVoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("CreatedById")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset?>("CreatedOn")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("InstrumentId")
                        .HasColumnType("int");

                    b.Property<bool>("IsMainVoice")
                        .HasColumnType("bit");

                    b.Property<int>("SongId")
                        .HasColumnType("int");

                    b.Property<int?>("UpdatedById")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset?>("UpdatedOn")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("VoiceName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VoiceNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("InstrumentId");

                    b.HasIndex("UpdatedById");

                    b.HasIndex("SongId", "VoiceNumber")
                        .IsUnique();

                    b.ToTable("SongVoices");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.SongSharedGroup", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<int>("SongId")
                        .HasColumnType("int");

                    b.HasKey("GroupId", "SongId");

                    b.HasIndex("SongId");

                    b.ToTable("SongSharedGroups");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.SongSharedOrganisation", b =>
                {
                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.Property<int>("SongId")
                        .HasColumnType("int");

                    b.HasKey("OrganisationId", "SongId");

                    b.HasIndex("SongId");

                    b.ToTable("SongSharedOrganisations");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.SongSharedUser", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("SongId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "SongId");

                    b.HasIndex("SongId");

                    b.ToTable("SongSharedUser");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsSystemAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("MsId")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("MsId")
                        .IsUnique()
                        .HasFilter("[MsId] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Group", b =>
                {
                    b.HasOne("Dissimilis.DbContext.Models.User", "CreatedBy")
                        .WithMany("GroupsCreated")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Dissimilis.DbContext.Models.Organisation", "Organisation")
                        .WithMany("Groups")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.GroupUser", b =>
                {
                    b.HasOne("Dissimilis.DbContext.Models.Group", "Group")
                        .WithMany("Users")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Dissimilis.DbContext.Models.User", "User")
                        .WithMany("Groups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Organisation", b =>
                {
                    b.HasOne("Dissimilis.DbContext.Models.User", "CreatedBy")
                        .WithMany("OrganisationsCreated")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.OrganisationUser", b =>
                {
                    b.HasOne("Dissimilis.DbContext.Models.Organisation", "Organisation")
                        .WithMany("Users")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Dissimilis.DbContext.Models.User", "User")
                        .WithMany("Organisations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Organisation");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Song.Song", b =>
                {
                    b.HasOne("Dissimilis.DbContext.Models.User", "Arranger")
                        .WithMany("SongsArranged")
                        .HasForeignKey("ArrangerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Dissimilis.DbContext.Models.User", "CreatedBy")
                        .WithMany("SongsCreated")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Dissimilis.DbContext.Models.User", "UpdatedBy")
                        .WithMany("SongsUpdated")
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Arranger");

                    b.Navigation("CreatedBy");

                    b.Navigation("UpdatedBy");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Song.SongBar", b =>
                {
                    b.HasOne("Dissimilis.DbContext.Models.Song.SongVoice", "SongVoice")
                        .WithMany("SongBars")
                        .HasForeignKey("SongVoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SongVoice");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Song.SongNote", b =>
                {
                    b.HasOne("Dissimilis.DbContext.Models.Song.SongBar", "SongBar")
                        .WithMany("Notes")
                        .HasForeignKey("BarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SongBar");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Song.SongVoice", b =>
                {
                    b.HasOne("Dissimilis.DbContext.Models.User", "CreatedBy")
                        .WithMany("SongVoiceCreated")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Dissimilis.DbContext.Models.Instrument", "Instrument")
                        .WithMany("SongVoices")
                        .HasForeignKey("InstrumentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Dissimilis.DbContext.Models.Song.Song", "Song")
                        .WithMany("Voices")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dissimilis.DbContext.Models.User", "UpdatedBy")
                        .WithMany("SongVoiceUpdated")
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("CreatedBy");

                    b.Navigation("Instrument");

                    b.Navigation("Song");

                    b.Navigation("UpdatedBy");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.SongSharedGroup", b =>
                {
                    b.HasOne("Dissimilis.DbContext.Models.Group", "Group")
                        .WithMany("SharedSongs")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Dissimilis.DbContext.Models.Song.Song", "Song")
                        .WithMany("SharedGroups")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.SongSharedOrganisation", b =>
                {
                    b.HasOne("Dissimilis.DbContext.Models.Organisation", "Organisation")
                        .WithMany("SharedSongs")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Dissimilis.DbContext.Models.Song.Song", "Song")
                        .WithMany("SharedOrganisations")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Organisation");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.SongSharedUser", b =>
                {
                    b.HasOne("Dissimilis.DbContext.Models.Song.Song", "Song")
                        .WithMany("SharedUsers")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Dissimilis.DbContext.Models.User", "User")
                        .WithMany("SongsShared")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Song");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.User", b =>
                {
                    b.HasOne("Dissimilis.DbContext.Models.Country", "Country")
                        .WithMany("Users")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Country");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Country", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Group", b =>
                {
                    b.Navigation("SharedSongs");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Instrument", b =>
                {
                    b.Navigation("SongVoices");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Organisation", b =>
                {
                    b.Navigation("Groups");

                    b.Navigation("SharedSongs");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Song.Song", b =>
                {
                    b.Navigation("SharedGroups");

                    b.Navigation("SharedOrganisations");

                    b.Navigation("SharedUsers");

                    b.Navigation("Voices");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Song.SongBar", b =>
                {
                    b.Navigation("Notes");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.Song.SongVoice", b =>
                {
                    b.Navigation("SongBars");
                });

            modelBuilder.Entity("Dissimilis.DbContext.Models.User", b =>
                {
                    b.Navigation("Groups");

                    b.Navigation("GroupsCreated");

                    b.Navigation("Organisations");

                    b.Navigation("OrganisationsCreated");

                    b.Navigation("SongsArranged");

                    b.Navigation("SongsCreated");

                    b.Navigation("SongsShared");

                    b.Navigation("SongsUpdated");

                    b.Navigation("SongVoiceCreated");

                    b.Navigation("SongVoiceUpdated");
                });
#pragma warning restore 612, 618
        }
    }
}
