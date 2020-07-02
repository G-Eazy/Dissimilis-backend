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
        protected readonly IUserService userService;

        //Create Database set for all the models
        public DbSet<User> Users { get; set; }
		public DbSet<Song> Songs { get; set; }
		public DbSet<Part> Part { get; set; }
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
			//Only ensure delete if in debug mode
/*			#if DEBUG
				this.Database.EnsureDeleted();
			#endif*/

			this.Database.EnsureCreated();
		}

		public DissimilisDbContext(DbContextOptions dbOptions, IUserService userService) : base (dbOptions)
        {
			this.userService = userService;
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

		/*This region builds all the models, BuildNAMEOFMODEL is
		 the naming convention.*/
		static void BuildUser (ModelBuilder builder)
        {
			var entity = builder.Entity<User>();

			//Set unique username
			entity.HasIndex(x => x.Username).IsUnique();

			//Set unique email
			entity.HasIndex(x => x.Email).IsUnique();

			//set one to many relationshop between Country and Users
			entity.HasOne(x => x.Country).WithMany()
				.HasForeignKey(x => x.CountryId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Restrict);

			entity.HasOne(x => x.Organisation).WithMany()
				.HasForeignKey(x => x.OrganisationId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
		}

		static void BuildSong (ModelBuilder builder)
        {
			var entity = builder.Entity<Song>();

		
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
				.HasForeignKey(x => x.SongId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
			//Set foregin key linked to Instrument and InstrumentId
			entity.HasOne(x => x.Instrument).WithMany()
				.HasForeignKey(x => x.InstrumentId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
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
				.HasForeignKey(x => x.PartId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);

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

			entity.HasIndex(x => x.Id).IsUnique();
        }

		static void BuildUserGroup(ModelBuilder builder)
		{
			var entity = builder.Entity<UserGroup>();

			entity.HasIndex(x => x.Id).IsUnique();
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

		public override int SaveChanges()
		{
			var entries = ChangeTracker
				.Entries()
				.Where(e => e.Entity is BaseEntity && (
						e.State == EntityState.Added
						|| e.State == EntityState.Modified 
						|| e.State == EntityState.Deleted));

			/*var IdentityName = 1;*/

			foreach (var item in entries)
			{
				((BaseEntity)item.Entity).CreatedOn = DateTime.Now;

				if (item.State == EntityState.Added)
				{
					((BaseEntity)item.Entity).UpdatedOn = DateTime.Now;
				}
			}

			return base.SaveChanges();
		}

	}
}