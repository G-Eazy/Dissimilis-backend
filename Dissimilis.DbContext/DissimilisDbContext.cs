using System;
using System.Linq;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dissimilis.DbContext
{
    public class DissimilisDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DissimilisDbContext(DbContextOptions<DissimilisDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //   optionsBuilder.EnableSensitiveDataLogging();
        }

        //Create Database set for all the models
        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<SongVoice> SongVoices { get; set; }
        public DbSet<SongBar> SongBars { get; set; }
        public DbSet<SongNote> SongNotes { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<OrganisationUser> OrganisationUsers { get; set; }
        public DbSet<SongGroupTag> SongGroupTags { get; set; }
        public DbSet<SongOrganisationTag> SongOrganisationTags { get; set; }
        public DbSet<SongSharedUser> SongSharedUser { get; set; }






        /// <summary>
        /// Created the models and configure them
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Here we are building the models and creating the different keys
            BuildUser(modelBuilder);
            BuildSong(modelBuilder);
            BuildSongVoice(modelBuilder);
            BuildInstrument(modelBuilder);
            BuildBar(modelBuilder);
            BuildNote(modelBuilder);
            BuildCountry(modelBuilder);
            BuildOrganisation(modelBuilder);
            BuildGroup(modelBuilder);
            BuildGroupUser(modelBuilder);
            BuildOrganisationUser(modelBuilder);
            BuildSongGroupTag(modelBuilder);
            BuildSongOrganisationTag(modelBuilder);
            BuildSongSharedUser(modelBuilder);

            FixForSqlLite(modelBuilder);
        }

        private void FixForSqlLite(ModelBuilder builder)
        {
            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                return;
            }

            // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
            // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
            // use the DateTimeOffsetToBinaryConverter
            // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
            // This only supports millisecond precision, but should be sufficient for most use cases.
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                    || p.PropertyType == typeof(DateTimeOffset?));
                foreach (var property in properties)
                {
                    builder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(new DateTimeOffsetToBinaryConverter());
                }
            }

        }

        #region builder for all the models
        /*This region builds all the models, BuildNAMEOFMODEL is
		 the naming convention.*/
        static void BuildUser(ModelBuilder builder)
        {
            var entity = builder.Entity<User>();

            //Set unique email
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasIndex(x => x.MsId).IsUnique();

            //set one to many relationshop between Country and Users
            entity.HasOne(x => x.Country)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        static void BuildSong(ModelBuilder builder)
        {
            var entity = builder.Entity<Song>();
            entity.Property(x => x.Title).IsRequired();

            entity.HasOne(x => x.Arranger)
                .WithMany(x => x.SongsArranged)
                .HasForeignKey(x => x.ArrangerId)
                 .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CreatedBy)
                .WithMany(x => x.SongsCreated)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.UpdatedBy)
                .WithMany(x => x.SongsUpdated)
                .HasForeignKey(x => x.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }

        static void BuildSongVoice(ModelBuilder builder)
        {
            var entity = builder.Entity<SongVoice>();

            entity.HasIndex(x => new
            {
                x.SongId,
                PartNumber = x.VoiceNumber
            }).IsUnique();

            //set foregin key for creator id
            entity.HasOne(x => x.Song)
                .WithMany(s => s.Voices)
                .HasForeignKey(x => x.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            //Set foregin key linked to Instrument and InstrumentId
            entity.HasOne(x => x.Instrument)
                .WithMany(x => x.SongVoices)
                .HasForeignKey(x => x.InstrumentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CreatedBy)
                .WithMany(x => x.SongVoiceCreated)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.UpdatedBy)
                .WithMany(x => x.SongVoiceUpdated)
                .HasForeignKey(x => x.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

        }

        static void BuildBar(ModelBuilder builder)
        {
            var entity = builder.Entity<SongBar>();

            //Set a unique Id for barnumber that is related to PartId
            //Each barnumber needs to be unique but only within it's
            //corresponding Part.
            entity.HasIndex(x => new { x.SongVoiceId, x.Position });

            //Set foregin key for PartId linked to the Id of Part
            entity.HasOne(x => x.SongVoice)
                .WithMany(x => x.SongBars)
                .HasForeignKey(x => x.SongVoiceId)
                .OnDelete(DeleteBehavior.Cascade);

        }

        static void BuildNote(ModelBuilder builder)
        {
            var entity = builder.Entity<SongNote>();

            //Set a unique Id for barnumber that is related to PartId
            //Each barnumber needs to be unique but only within it's
            //corresponding Part.
            entity.HasIndex(x => new { x.BarId, NoteNumber = x.Position }).IsUnique();

            //Set foregin key for PartId linked to the Id of Part
            entity.HasOne(x => x.SongBar)
                .WithMany(x => x.Notes)
                .HasForeignKey(x => x.BarId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        static void BuildInstrument(ModelBuilder builder)
        {
            var entity = builder.Entity<Instrument>();
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).IsRequired();
        }

        static void BuildCountry(ModelBuilder builder)
        {
            var entity = builder.Entity<Country>();

            entity.HasIndex(x => x.Name).IsUnique();
        }


        static void BuildOrganisation(ModelBuilder builder)
        {
            var entity = builder.Entity<Organisation>();

            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).IsRequired();


            entity.HasOne(x => x.CreatedBy)
                .WithMany(x => x.OrganisationsCreated)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

        }
        static void BuildGroup(ModelBuilder builder)
        {
            var entity = builder.Entity<Group>();
            entity.Property(x => x.Name).IsRequired();

            entity.HasOne(x => x.Organisation)
                .WithMany(x => x.Groups)
                .HasForeignKey(x => x.OrganisationId)
                 .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.CreatedBy)
                .WithMany(x => x.GroupsCreated)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

        }
        static void BuildGroupUser(ModelBuilder builder)
        {
            var entity = builder.Entity<GroupUser>();
            entity.HasKey(x => new { x.UserId, x.GroupId });


            entity.HasOne(x => x.Group)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.GroupId)
                 .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Groups)
                .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
        static void BuildOrganisationUser(ModelBuilder builder)
        {
            var entity = builder.Entity<OrganisationUser>();
            entity.HasKey(x => new { x.UserId, x.OrganisationId });


            entity.HasOne(x => x.Organisation)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.OrganisationId)
                 .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Organisations)
                .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
        static void BuildSongGroupTag(ModelBuilder builder)
        {
            var entity = builder.Entity<SongGroupTag>();
            entity.HasKey(x => new { x.GroupId, x.SongId });


            entity.HasOne(x => x.Group)
                .WithMany(x => x.SharedSongs)
                .HasForeignKey(x => x.GroupId)
                 .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Song)
                .WithMany(x => x.GroupTags)
                .HasForeignKey(x => x.SongId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
        static void BuildSongOrganisationTag(ModelBuilder builder)
        {
            var entity = builder.Entity<SongOrganisationTag>();
            entity.HasKey(x => new { x.OrganisationId, x.SongId });


            entity.HasOne(x => x.Organisation)
                .WithMany(x => x.SharedSongs)
                .HasForeignKey(x => x.OrganisationId)
                 .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Song)
                .WithMany(x => x.OrganisationTags)
                .HasForeignKey(x => x.SongId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
        static void BuildSongSharedUser(ModelBuilder builder)
        {
            var entity = builder.Entity<SongSharedUser>();
            entity.HasKey(x => new { x.UserId, x.SongId });

            entity.HasOne(x => x.User)
                .WithMany(x => x.SongsShared)
                .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Song)
                .WithMany(x => x.SharedUsers)
                .HasForeignKey(x => x.SongId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
        #endregion

}

