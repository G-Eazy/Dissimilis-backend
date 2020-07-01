using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Dissimilis.WebAPI.Database;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Database
{
    public class DissimilisDbContext : DbContext
	{
		//Create Database set for all the models
		public DbSet<User> Users { get; set; }
		public DbSet<Song> Songs { get; set; }
		public DbSet<Part> Part { get; set; }
		public DbSet<Resource> Resources { get; set; }
		public DbSet<Bar> Bars { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<Instrument> Instruments { get; set; }
		public DbSet<UserGroup> UserGroups { get; set; }

		public DissimilisDbContext() : base(new DissimilisDbContextOptions().Options)
		{
			//Only ensure delete if in debug mode
			#if DEBUG
				this.Database.EnsureDeleted();
			#endif

			this.Database.EnsureCreated();
		}

		public DissimilisDbContext(DbContextOptions dbOptions) : base (dbOptions)
        {
			//Empty constructor with parameters to be used in startup.cs
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//Empty constructor to be used later for configuring the dbcontext
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//Here we are building the models and creating the different keys
			#region model builder for User
			{
				var entity = modelBuilder.Entity<User>();

				//Set unique ID for music sheet
				entity.HasIndex(x => x.Id).IsUnique();

				//Set unique username
				entity.HasIndex(x => x.Username).IsUnique();

				//Set unique email
				entity.HasIndex(x => x.Email).IsUnique();

				//set one to many relationshop between Country and Users
				entity.HasOne(x => x.Country).WithMany()
					.HasForeignKey(x => x.CountryId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Restrict);

				//Set one to many relationshop between UserGroup and Users
				entity.HasOne(x => x.UserGrp).WithMany()
					.HasForeignKey(x => x.UserGrpId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Restrict);
			}
			#endregion


			//insert some new data
			#region Model builder for Song
			{
				var entity = modelBuilder.Entity<Song>();

				//set unique ID
				entity.HasIndex(x => x.CreatorId).IsUnique();

				//set foregin key for creator id
				entity.HasOne(x => x.Creator).WithMany()
					.HasForeignKey(x => x.CreatorId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Restrict);
			}
			#endregion

			//insert some new data
			#region Model builder for Part
			{
				var entity = modelBuilder.Entity<Part>();

				//set unique ID
				entity.HasIndex(x => x.Id).IsUnique();

				//set foregin key for creator id
				entity.HasOne(x => x.Song).WithMany()
					.HasForeignKey(x => x.SongId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Restrict);

				//Set foregin key linked to Instrument and InstrumentId
				entity.HasOne(x => x.Instrument).WithMany()
					.HasForeignKey(x => x.InstrumentId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);

			}
			#endregion

			#region Instrument data model
			{
				var entity = modelBuilder.Entity<Instrument>();

				//Set instrument.Id to be unique
				entity.HasIndex(x => x.Id).IsUnique();

			}
            #endregion

            #region bar entity
            {
				var entity = modelBuilder.Entity<Bar>();

				//Set a unique Id for barnumber that is related to PartId
				//Each barnumber needs to be unique but only within it's
				//corresponding Part.
				entity.HasIndex(x => new
				{
					x.PartId, x.BarNumber
				});

				//Set foregin key for PartId linked to the Id of Part
				entity.HasOne(x => x.Part).WithMany()
					.HasForeignKey(x => x.PartId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);


            }
            #endregion


        }
    }
}