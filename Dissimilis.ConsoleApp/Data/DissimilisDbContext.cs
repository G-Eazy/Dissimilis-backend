using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Dissimilis.ConsoleApp.Database.Models;
using Dissimilis.ConsoleApp.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Dissimilis.ConsoleApp.Database
{
	public class DissimilisDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Song> Songs { get; set; }
		public DbSet<Part> Part { get; set; }
		public DbSet<Resource> Resources { get; set; }

		public DissimilisDbContext() : base(new DissimilisDbContextOptions().Options)
		{
			this.Database.EnsureDeleted();
			this.Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);


			#region model builder for User
			{
				var entity = modelBuilder.Entity<User>();

				//Set unique ID for music sheet
				entity.HasIndex(x => x.UserId).IsUnique();

				//Set unique username
				entity.HasIndex(x => x.Username).IsUnique();

				//Set unique email
				entity.HasIndex(x => x.Email).IsUnique();

				//set one to many relationshop between Country and Users
				entity.HasOne(x => x.Country).WithMany(x => x.Users)
					.HasForeignKey(x => x.CountryId).HasPrincipalKey(x => x.CountryId).OnDelete(DeleteBehavior.Restrict);

				//Set one to many relationshop between UserGroup and Users
				entity.HasOne(x => x.UserGroup).WithMany(x => x.Users)
					.HasForeignKey(x => x.UserGroupId).HasPrincipalKey(x => x.UserGroupId).OnDelete(DeleteBehavior.Restrict);
			}
			#endregion


			//insert some new data
			#region Model builder for Song
			{
				var entity = modelBuilder.Entity<Song>();

				//set unique ID
				entity.HasIndex(x => x.CreatorId).IsUnique();

				//set foregin key for creator id
				entity.HasOne(x => x.Creator).WithMany(x => x.Songs)
					.HasForeignKey(x => x.CreatorId).HasPrincipalKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            }
			#endregion

			//insert some new data
			#region Model builder for Part
			{
				var entity = modelBuilder.Entity<Part>();

				//set unique ID
				entity.HasIndex(x => x.PartId).IsUnique();

				//set foregin key for creator id
				entity.HasOne(x => x.Song).WithMany(x => x.Parts)
					.HasForeignKey(x => x.SongId).HasPrincipalKey(x => x.SongId).OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(x => x.Instrument).WithMany(x => x.Parts)
					.HasForeignKey(x => x.Instrument);
			
			}
            #endregion

            #region Instrument data model
            {
				var entity = modelBuilder.Entity<Instrument>();

				entity.HasIndex(x => x.InstrumentId).IsUnique();

            }
			#endregion


		}
    }
}