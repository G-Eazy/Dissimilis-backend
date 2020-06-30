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
			#if DEBUG
				this.Database.EnsureDeleted();
			#endif

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
				entity.HasIndex(x => x.ID).IsUnique();

				//Set unique username
				entity.HasIndex(x => x.Username).IsUnique();

				//Set unique email
				entity.HasIndex(x => x.Email).IsUnique();

				//set one to many relationshop between Country and Users
				entity.HasOne(x => x.Country).WithMany()
					.HasForeignKey(x => x.CountryId).HasPrincipalKey(x => x.ID).OnDelete(DeleteBehavior.Restrict);

				//Set one to many relationshop between UserGroup and Users
				entity.HasOne(x => x.UserGroup).WithMany()
					.HasForeignKey(x => x.UserGroupId).HasPrincipalKey(x => x.ID).OnDelete(DeleteBehavior.Restrict);
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
					.HasForeignKey(x => x.CreatorId).HasPrincipalKey(x => x.ID).OnDelete(DeleteBehavior.Restrict);
			}
			#endregion

			//insert some new data
			#region Model builder for Part
			{
				var entity = modelBuilder.Entity<Part>();

				//set unique ID
				entity.HasIndex(x => x.ID).IsUnique();

				//set foregin key for creator id
				entity.HasOne(x => x.Song).WithMany()
					.HasForeignKey(x => x.SongId).HasPrincipalKey(x => x.ID).OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(x => x.Instrument).WithMany()
					.HasForeignKey(x => x.Instrument);

			}
			#endregion

			#region Instrument data model
			{
				var entity = modelBuilder.Entity<Instrument>();

				entity.HasIndex(x => x.ID).IsUnique();

			}
            #endregion

            #region bar entity
            {
				var entity = modelBuilder.Entity<Bar>();

				entity.HasIndex(x => new
				{
					x.PartId, x.BarNumber
				});

				entity.HasOne(x => x.Part).WithMany()
					.HasForeignKey(x => x.PartId).HasPrincipalKey(x => x.ID).OnDelete(DeleteBehavior.Cascade);


            }
            #endregion


        }
    }
}