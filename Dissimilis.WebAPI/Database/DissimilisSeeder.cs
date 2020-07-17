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

			var User = context.Users.SingleOrDefault(x => x.Name == "Bård Bjørge");
			if (User is null)
			{
				context.Users.Add(new User() { Name = "AdminUser", Email = "admin@support.no" });
				context.Users.Add(new User() { Name = "Bård Bjørge", Email = "bård@dissimilis.no" });
			}

			context.SaveChanges();
			
			//Check if there is any data called Admin in the database first
			var Instructor = context.UserGroups.FirstOrDefault(b => b.Name == "Admin");

			//If null, add a usergroup
			if (Instructor == null)
			{
				//Add some different groups, new users that will automatically be guest
				context.UserGroups.Add(new UserGroup("Admin"));
				context.UserGroups.Add(new UserGroup("User"));
				context.UserGroups.Add(new UserGroup("Instructor"));
			}

			var Resources = context.Resources.FirstOrDefault(x => x.Name == "Read");
			if (Resources is null)
            {
				//Add some different kinds of resources to access
				context.Resources.Add(new Resource() { Name = "Read" });
				context.Resources.Add(new Resource() { Name = "Write" });
				context.Resources.Add(new Resource() { Name = "Delete" });
				context.Resources.Add(new Resource() { Name = "Copy" });
				context.Resources.Add(new Resource() { Name = "Update" });
            }

			var Organisation = context.Organisations.SingleOrDefault(x => x.Name == "Dissimilis Norge");
			if (Organisation is null)
            {
				context.Organisations.Add(new Organisation("Ukjent"));
				context.Organisations.Add(new Organisation("Dissimilis Norge"));
				context.Organisations.Add(new Organisation("Dissimilis Kultursenter"));
			}

			var Norway = context.Countries.SingleOrDefault(x => x.Name == "Norge");
			if (Norway is null)
			{
				context.Countries.Add(new Country("Norge"));
				context.Countries.Add(new Country("Sverige"));
			}

			var Instrument = context.Instruments.SingleOrDefault(x => x.Name == "Piano");
			if (Instrument is null)
			{
				context.Instruments.Add(new Instrument("Piano"));
				context.Instruments.Add(new Instrument("Gitar"));
				context.Instruments.Add(new Instrument("Bass"));
			}

			context.UserId = 1;
			context.SaveChanges();

			var Baard = context.Users.SingleOrDefault(x => x.Name == "Bård Bjørge");
			if (Baard.OrganisationId is null)
			{
				Baard.OrganisationId = 1;
				Baard.CountryId = 1;
				var Admin = context.Users.SingleOrDefault(x => x.Name == "AdminUser");
				Admin.OrganisationId = 1;
				Admin.CountryId = 1;
			}

			//Insert the many to many enteties as the ones above need to have been
			//saved first
			var resourceGroup = context.UserGroupResources.SingleOrDefault(x => x.ResourceId == 1);
			if (resourceGroup is null)
			{
				//Add the resources to the group they belong to 
				//ADMIN:
				context.UserGroupResources.Add(new UserGroupResources(1, 1));
				context.UserGroupResources.Add(new UserGroupResources(2, 1));
				context.UserGroupResources.Add(new UserGroupResources(3, 1));
				context.UserGroupResources.Add(new UserGroupResources(4, 1));
				context.UserGroupResources.Add(new UserGroupResources(5, 1));
				
				//GUEST:
				context.UserGroupResources.Add(new UserGroupResources(1, 2));
				
				//INSTRUCTOR:
				context.UserGroupResources.Add(new UserGroupResources(1, 3));
				context.UserGroupResources.Add(new UserGroupResources(2, 3));
				context.UserGroupResources.Add(new UserGroupResources(4, 3));
				context.UserGroupResources.Add(new UserGroupResources(5, 3));

			}

			var MemberGroup = context.UserGroupMembers.SingleOrDefault(x => x.UserId == 1);
			if (MemberGroup is null)
			{
				context.UserGroupMembers.Add(new UserGroupMembers(1, 1));
			}

			var FirstSong = context.Songs.SingleOrDefault(x => x.Title == "Lisa Gikk Til Skolen");
			if (FirstSong is null)
            {
				context.Songs.Add(new Song() { Title = "Lisa Gikk Til Skolen", Composer = "Unknown", ArrangerId = 1, TimeSignature = "4/4" });
				context.Songs.Add(new Song() { Title = "Fade To Black", Composer = "Metallica", ArrangerId = 1, TimeSignature = "4/4" });
				context.Songs.Add(new Song() { Title = "Be Yourself", Composer = "Audioslave", ArrangerId = 1, TimeSignature = "4/4" });
			}

			context.UserId = 1;
			context.SaveChanges();

		}
	}
}
