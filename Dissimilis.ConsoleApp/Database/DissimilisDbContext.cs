using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Dissimilis.ConsoleApp.Database.Models;
using Dissimilis.ConsoleApp.Database;

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


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);


			#region initalize two users
			{
				var entity = modelBuilder.Entity<User>();

				//Set unique ID for music sheet
				entity.HasIndex(x => x.UserId).IsUnique();

				//Set unique username
				entity.HasIndex(x => x.Username).IsUnique();

				//set foregin key for countrycode
				entity.HasOne(x => x.Country).WithMany()
					.HasForeignKey(x => x.CountryId).HasPrincipalKey(x => x.ID).OnDelete(DeleteBehavior.Restrict);

				//Set foregin key for usergroup
				entity.HasOne(x => x.UserGroup).WithMany()
					.HasForeignKey(x => x.UserGroupId).HasPrincipalKey(x => x.UserGroupId).OnDelete(DeleteBehavior.Restrict);
			}
			#endregion


			//insert some new data
			#region initalize a music sheet
			{
				var entity = modelBuilder.Entity<Song>();

				//set unique ID
				entity.HasIndex(x => x.CreatorId).IsUnique();

				//set foregin key for creator id
				entity.HasOne(x => x.Creator).WithMany()
					.HasForeignKey(x => x.CreatorId).HasPrincipalKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            }
			#endregion

			/*
			DateOfBirth = new DateTime(2000, 01, 01)
			{
				var entity = modelBuilder.Entity<Product>();

				entity.HasIndex(x => x.ProductName).IsUnique();

				entity.HasOne(x => x.Brand).WithMany()
					.HasForeignKey(x => x.BrandID).HasPrincipalKey(x => x.ID).OnDelete(DeleteBehavior.Restrict);
			}
			#endregion

			#region Handle brands
			{
				var entity = modelBuilder.Entity<Brand>();

				entity.HasIndex(x => x.BrandName).IsUnique();
			}
			#endregion
			*/
		}
	}
}