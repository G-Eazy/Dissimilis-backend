using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dissimilis.WebAPI.Database.Models;

namespace Dissimilis.WebAPI.Database
{
    static class DissimilisSeeder 
	{
		public static void SeedData(DissimilisDbContext context)
        {
			context.Database.EnsureCreated();

			//Check if there is any data called Admin in the database first
			var UserGroup1 = context.UserGroups.FirstOrDefault(b => b.GroupName == "Admin");
				
			//If null, add a usergroup
			if (UserGroup1 == null)
			{
				context.UserGroups.Add(new UserGroup("Admin"));
			}

			context.SaveChanges();
			
		}
		///Values used to initialize the DB, not sure where to put it, this function 
		///doesnt work as it can't use var for some reason.
		///
		/*
		 var cNo = new Country("Norway", "No");
				var cSwe = new Country("Sweden", "Se");
				var gAdmin = new UserGroup("Admin", "Adm1");
				this.Add(cNo);
				this.Add(cSwe);
				this.Add(gAdmin);
				this.SaveChanges();

		 var user1 = new User
				(
					"Atle_diss",
					"Atle",
					"atle@diss.no",
					new UserGroup("instructor"),
					new Country("NO"),
					new DateTime(1994, 10, 06)
				);

		var user2 = new User
			(
				"Bård_diss",
				"Bård",
				"Bård@diss.no",
				new UserGroup("admin"),
				new Country("NO"),
				new DateTime(1984, 02, 07)
			);

			context.Add(user1);
			context.Add(user2);

			var musicSheet = new MusicSheet(
						"Song1",
						user1,
						"A bunch of abc notation text goes here",
						"A composer",
						"4/4",
						"1/8",
						new DateTime(2019, 07, 23)
					);

			context.Add(musicSheet);
			context.SaveChanges();
		*/


		/*
		 * var test = this.Users.FirstOrDefault(x => x.ID == 1);

			if (test == null)
			{
				var user1 = new User
					(
						"Atle_diss",
						"Atle",
						"atle@diss.no",
						new UserGroup() { GroupName = "Admin", ResourceGroup = "Adm1"},
						new Country() { CountryCode = "No", CountryName = "Norway" },
						new DateTime(1994, 10, 06)
					);

				var user2 = new User
					(
						"Bård_diss",
						"Bård",
						"Bård@diss.no",
						new UserGroup() { GroupName = "Admin", ResourceGroup = "Adm1" },
						new Country() { CountryCode = "Swe", CountryName = "Sweden" },
						new DateTime(1984, 02, 07)
					);

				this.Add(user1);
				this.Add(user2);

				var musicSheet = new MusicSheet(
							"Song1",
							user1,
							"A bunch of abc notation text goes here",
							"A composer",
							"4/4",
							"1/8",
							new DateTime(2019, 07, 23)
						);

				this.Add(musicSheet);
				this.SaveChanges();
				}







			modelBuilder.Entity<Country>().HasData(new Country { ID = 1, CountryCode = "No", CountryName = "Norway" });
			modelBuilder.Entity<Country>().HasData(new Country { ID = 2, CountryCode = "Swe", CountryName = "Sweden" });
			modelBuilder.Entity<UserGroup>().HasData(new UserGroup { ID = 1, GroupName = "Admin", ResourceGroup = "Adm1" });

			modelBuilder.Entity<User>().HasData(
							new User()
							{
								UserId = 1,
								Username = "Atle_diss",
								Name = "Atle",
								Email = "atle@diss.no",
								CountryId = 1,
								UserGroupId = 1,
								DateOfBirth = new DateTime(1994, 10, 06)
							},
							new User()
							{
								UserId = 2,
								Username = "Bård_diss",
								Name = "Bård",
								Email = "bård@diss.no",
								CountryId = 2,
								UserGroupId = 1,
								DateOfBirth = new DateTime(1974, 11, 03)
							}
					);

			modelBuilder.Entity<MusicSheet>().HasData(
				new MusicSheet()
				{
					ID = 1,
					Title = "Song1",
					Notation = "A bunch of abc notation text goes here",
					Composer = "A composer",
					Tact = "4/4",
					TimeSignature = "1/8",
					CreationTime = new DateTime(2019, 07, 23)
				}
			);
			*/
	}
}
