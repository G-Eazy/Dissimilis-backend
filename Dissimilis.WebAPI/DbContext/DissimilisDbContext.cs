﻿using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Dissimilis.WebAPI.Database;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Dissimilis.WebAPI.Database.Audit;

namespace Dissimilis.WebAPI.Database
{
    public class DissimilisDbContext : DbContext
	{
        protected readonly IUserService UserService;

        //Create Database set for all the models
        public DbSet<User> Users { get; set; }
		public DbSet<Song> Songs { get; set; }
		public DbSet<Part> Parts { get; set; }
		public DbSet<Resource> Resources { get; set; }
		public DbSet<Bar> Bars { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<Instrument> Instruments { get; set; }
		public DbSet<UserGroup> UserGroups { get; set; }
		public DbSet<Organisation> Organisations { get; set; }
		public DbSet<UserGroupMembers> UserGroupMembers { get; set; }
		public DbSet<UserGroupResources> UserGroupResources { get; set; }


		public DissimilisDbContext() : base(new DissimilisDbContextOptions().Options)
		{
			//Only ensure delete if in debug mode, commented out as it isn't working right now
			/*#if DEBUG
				this.Database.EnsureDeleted();
			#endif*/
			//this.Database.Migrate();
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
			BuildCountry(modelBuilder);
			BuildOrganisation(modelBuilder);
			BuildUserGroup(modelBuilder);
			BuildUserGroupMembers(modelBuilder);
			BuildResources(modelBuilder);
			BuildUserGroupResource(modelBuilder);
		}


		#region builder for all the models
		/*This region builds all the models, BuildNAMEOFMODEL is
		 the naming convention.*/
		static void BuildUser(ModelBuilder builder)
		{
			var entity = builder.Entity<User>();

			//Set unique username
			//entity.HasIndex(x => x.Username).IsUnique();

			//Set unique email
			entity.HasIndex(x => x.Email).IsUnique();

			//set one to many relationshop between Country and Users
			entity.HasOne(x => x.Country).WithMany()
				.HasForeignKey(x => x.CountryId);

			entity.HasOne(x => x.Organisation).WithMany()
				.HasForeignKey(x => x.OrganisationId);

		}

		static void BuildSong (ModelBuilder builder)
        {
			var entity = builder.Entity<Song>();
			entity.HasOne(x => x.Arranger).WithMany().HasForeignKey(x => x.ArrangerId);
		}
	
		static void BuildPart (ModelBuilder builder)
        {
			var entity = builder.Entity<Part>();

			entity.HasIndex(x => new
			{
				x.SongId,
				x.PartNumber
			});

			//set foregin key for creator id
			entity.HasOne(x => x.Song).WithMany()
				.HasForeignKey(x => x.SongId);
			//Set foregin key linked to Instrument and InstrumentId
			entity.HasOne(x => x.Instrument).WithMany()
				.HasForeignKey(x => x.InstrumentId);

		}

		static void BuildBar(ModelBuilder builder)
		{
			var entity = builder.Entity<Bar>();

			//Set a unique Id for barnumber that is related to PartId
			//Each barnumber needs to be unique but only within it's
			//corresponding Part.
			entity.HasIndex(x => new
			{
				x.PartId,
				x.BarNumber
			});

			//Set foregin key for PartId linked to the Id of Part
			entity.HasOne(x => x.Part).WithMany()
				.HasForeignKey(x => x.PartId);

		}

		static void BuildInstrument (ModelBuilder builder)
        {
			var entity = builder.Entity<Instrument>();

			//Set instrument.Id to be unique
			entity.HasIndex(x => x.Name).IsUnique();

		}
		
		static void BuildCountry (ModelBuilder builder)
        {
			var entity = builder.Entity<Country>();

			entity.HasIndex(x => x.Name).IsUnique();

		}

		static void BuildUserGroup(ModelBuilder builder)
		{
			var entity = builder.Entity<UserGroup>();

			entity.HasIndex(x => x.Name).IsUnique();

		}

		static void BuildResources(ModelBuilder builder)
		{
			var entity = builder.Entity<Resource>();

			entity.HasIndex(x => x.Name).IsUnique();

		}

		static void BuildOrganisation(ModelBuilder builder)
		{
			var entity = builder.Entity<Organisation>();

			entity.HasIndex(x => x.Name).IsUnique();

		}

		static void BuildUserGroupMembers(ModelBuilder builder)
		{
			var entity = builder.Entity<UserGroupMembers>();
			entity.HasKey(fk => new { fk.UserId, fk.UserGroupId});

            entity
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
            entity
                .HasOne(x => x.UserGroup)
                .WithMany()
                .HasForeignKey(x => x.UserGroupId)
                .HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
        }

		static void BuildUserGroupResource(ModelBuilder builder)
		{
			var entity = builder.Entity<UserGroupResources>();

			entity.HasKey(fk => new { fk.ResourceId, fk.UserGroupId });

            entity
                .HasOne(x => x.UserGroup)
                .WithMany()
                .HasForeignKey(x => x.UserGroupId)
                .HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
            entity
                .HasOne(x => x.Resource)
                .WithMany()
                .HasForeignKey(x => x.ResourceId)
                .HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
        }

        #endregion 

		/// <summary>
		/// Ocerrideing the savechanges to add modified and added date
		/// </summary>
		/// <returns></returns>
        public override int SaveChanges()
		{
			var entries = ChangeTracker
				.Entries()
				.Where(e =>	
				e.State == EntityState.Added
						|| e.State == EntityState.Modified 
						|| e.State == EntityState.Deleted);

            var UserIdentity = "System";

			foreach (var item in entries)
			{
				if (item.Entity is BaseEntity entity)
				{
					if (item.State == EntityState.Added)
					{
						((BaseEntity)item.Entity).CreatedOn = DateTime.Now;
						((BaseEntity)item.Entity).CreatedBy = UserIdentity;
					}

					if (item.State == EntityState.Modified)
					{
						((BaseEntity)item.Entity).UpdatedOn = DateTime.Now;
						((BaseEntity)item.Entity).UpdatedBy = UserIdentity; ;
					}
				}
			}

			return base.SaveChanges();
		}

	}
}
