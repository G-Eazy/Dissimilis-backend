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
			var Instructor = context.UserGroups.FirstOrDefault(b => b.Name == "Admin");

			//If null, add a usergroup
			if (Instructor == null)
			{
				context.UserGroups.Add(new UserGroup("Admin"));
			}

			var Organisation = context.Organisations.FirstOrDefault(x => x.Name == "Dissimilis Norge");
			if (Organisation is null)
            {
				context.Organisations.Add(new Organisation("Dissimilis Norge"));
            }

			var Sweden = context.Countries.FirstOrDefault(x => x.Name == "Norway");
			if (Sweden is null)
			{
				context.Countries.Add(new Country("Norway"));
			}

			var AccessRight = context.Resources.FirstOrDefault(x => x.Name == "All");
			if (AccessRight is null)
			{
				context.Resources.Add(new Resource("All"));
			}

			var User = context.Users.FirstOrDefault(x => x.Name == "Bård B.");
			if (User is null)
			{
				context.Users.Add(new User("Bård B.", "bård@dissimilis.no", 1, 1, new DateTime(1975, 08, 21)));
			}

			var Instrument = context.Instruments.FirstOrDefault(x => x.Name == "Piano");
			if (Instrument is null)
			{
				context.Instruments.Add(new Instrument("Piano"));
			}

			context.SaveChanges();

			//Insert the many to many enteties as the ones above need to have been
			//saved first
			var resourceGroup = context.UserGroupResources.FirstOrDefault(x => x.ResourceId == 1);
			if (resourceGroup is null)
			{
				context.UserGroupResources.Add(new UserGroupResources(1, 1));
			}

			var MemberGroup = context.UserGroupMembers.FirstOrDefault(x => x.UserId == 1);
			if (MemberGroup is null)
			{
				context.UserGroupMembers.Add(new UserGroupMembers(1, 1));
			}

			context.SaveChanges();

		}
	}
}
