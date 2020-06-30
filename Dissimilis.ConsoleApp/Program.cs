using Dissimilis.WebAPI.Database;
using System;
using Dissimilis.WebAPI.Database.Models;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI
{
	class Program
	{
		static void Main(string[] args)
		{
			//Hello line to console
			Console.WriteLine("Hello Dissimilis!");

			var context = new DissimilisDbContext();

			//Set up initial database values
			var cNo = new Country("Norway", "Norwegian");
			var cSwe = new Country("Sweden", "Swedish");
			var gAdmin = new UserGroup("Admin");
			context.Add(cNo);
			context.Add(cSwe);
			context.Add(gAdmin);
			context.SaveChanges();

			var user1 = new User(
						"Atle_diss",
						"Atle",
						"atle@diss.no",
						gAdmin,
						cNo,
						new DateTime(1994, 10, 06)
					);

			var user2 = new User(
						"Bård_diss",
						"Bård",
						"Bård@diss.no",
						gAdmin,
						cSwe,
						new DateTime(1984, 02, 07)
					);

			context.Add(user1);
			context.Add(user2);

			var score = new Song(
						"Song1",
						user1,
						"A composer",
						"4/4",
						new DateTime(2019, 07, 23)
					);

			context.Add(score);
			context.SaveChanges();


			Console.WriteLine("Write the name of a user to get info: ");
			var user = Console.ReadLine();

			while (user != "Q")
			{
				PrintUser(user, context);
				Console.WriteLine("Write another name for other users or 'Q' to quit: ");
				user = Console.ReadLine();
			}

			//exit terminal when user is done
			System.Environment.Exit(0);
		}

		static public void PrintUser(string user, DissimilisDbContext context)
		{
			var query = context.Users.Where(x => x.Name == user);

			if (query.Count() == 1)
			{
				var person = query.Single();
				Console.WriteLine("The user " + user + " has the following details: "
					+ "\n Username: " + person.Username
					+ "\n Country: " + person.Country.CountryName
					+ "\n Usergroup: " + person.UserGroup.GroupName
					+ "\n Email: " + person.Email
					+ "\n ID: " + person.ID + "\n");

			}
			else
			{
				Console.WriteLine("There doesn't exist a user with this name :(");
			}
		}
	}
}

