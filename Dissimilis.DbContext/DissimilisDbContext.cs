using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<SongVoice> SongParts { get; set; }
        public DbSet<SongBar> SongBars { get; set; }
        public DbSet<SongNote> SongNotes { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
     

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
            BuildPart(modelBuilder);
            BuildInstrument(modelBuilder);
            BuildBar(modelBuilder);
            BuildNote(modelBuilder);
            BuildCountry(modelBuilder);
            BuildOrganisation(modelBuilder);
            
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
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Organisation)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.OrganisationId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

        }

        static void BuildSong(ModelBuilder builder)
        {
            var entity = builder.Entity<Song>();
            entity.Property(x => x.Title).IsRequired();

            entity.HasOne(x => x.Arranger)
                .WithMany()
                .HasForeignKey(x => x.ArrangerId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CreatedBy)
                .WithMany(x => x.SongsCreated)
                .HasForeignKey(x => x.CreatedById)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.UpdatedBy)
                .WithMany(x => x.SongsUpdated)
                .HasForeignKey(x => x.UpdatedById)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);
        }

        static void BuildPart(ModelBuilder builder)
        {
            var entity = builder.Entity<SongVoice>();

            entity.HasIndex(x => new
            {
                x.SongId,
                x.PartNumber
            }).IsUnique();

            //set foregin key for creator id
            entity.HasOne(x => x.Song)
                .WithMany(s => s.Parts)
                .HasForeignKey(x => x.SongId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            //Set foregin key linked to Instrument and InstrumentId
            entity.HasOne(x => x.Instrument)
                .WithMany(x => x.Parts)
                .HasForeignKey(x => x.InstrumentId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CreatedBy)
                .WithMany(x => x.PartsCreated)
                .HasForeignKey(x => x.CreatedById)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.UpdatedBy)
                .WithMany(x => x.PartsUpdated)
                .HasForeignKey(x => x.UpdatedById)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

        }

        static void BuildBar(ModelBuilder builder)
        {
            var entity = builder.Entity<SongBar>();

            //Set a unique Id for barnumber that is related to PartId
            //Each barnumber needs to be unique but only within it's
            //corresponding Part.
            entity.HasIndex(x => new { x.PartId, x.BarNumber }).IsUnique();

            //Set foregin key for PartId linked to the Id of Part
            entity.HasOne(x => x.SongVoice)
                .WithMany()
                .HasForeignKey(x => x.PartId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

        }

        static void BuildNote(ModelBuilder builder)
        {
            var entity = builder.Entity<SongNote>();

            //Set a unique Id for barnumber that is related to PartId
            //Each barnumber needs to be unique but only within it's
            //corresponding Part.
            entity.HasIndex(x => new { x.BarId, x.NoteNumber }).IsUnique();

            //Set foregin key for PartId linked to the Id of Part
            entity.HasOne(x => x.SongBar)
                .WithMany()
                .HasForeignKey(x => x.BarId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);


        }

        static void BuildInstrument(ModelBuilder builder)
        {
            var entity = builder.Entity<Instrument>();

            //Set instrument.Id to be unique
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).IsRequired();

        }

        static void BuildCountry(ModelBuilder builder)
        {
            var entity = builder.Entity<Country>();

            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).IsRequired();

        }

     
        static void BuildOrganisation(ModelBuilder builder)
        {
            var entity = builder.Entity<Organisation>();

            entity.HasIndex(x => x.Name).IsUnique();

        }

     
        #endregion


    }
}
