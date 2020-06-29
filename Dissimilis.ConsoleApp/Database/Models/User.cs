using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.ConsoleApp.Database.Models
{
    public class User
    {
		[Key]
		public int UserId { get; set; }

		/// <summary>
		/// User name of logged in user
		/// </summary>
		[Required]
		public string Username { get; set; }

		/// <summary>
		/// Email address of user
		/// </summary>
		[Required]
		public string Email { get; set; }

		/// <summary>
		/// Name of the user
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// The user group this user belongs to (eg. admin)
		/// </summary>
		public UserGroup UserGroup { get; }
		public int UserGroupId { get; set;  }

		/// <summary>
		/// Date of birth of the user
		/// </summary>
		public DateTime? DateOfBirth { get; set; }

		/// <summary>
		/// Country the user is from or belongs to
		/// </summary>
		public Country Country { get; set; }
		public int CountryId { get; set; }


		public User() { }

		public User(string username, string name, string email, 
					UserGroup usergroup, Country country, DateTime? datetime)
		{
			this.Username = username;
			this.Email = email;
			this.Name = name;
			this.UserGroup = usergroup;
			this.Country = country;
		}
	}
}
