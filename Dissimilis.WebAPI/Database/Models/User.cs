﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Dissimilis.WebAPI.Database.Models
{
	public class User : BaseEntity
	{
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// User name of logged in user
		/// </summary>
		[Required]
		public string Username { get; set; }

		[PasswordPropertyText]
		public string Password { get; set; }

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
		/// Date of birth of the user
		/// </summary>
		public DateTime? DateOfBirth { get; set; }

		/// <summary>
		/// Country the user is from or belongs to
		/// </summary>
		public Country Country { get; set; }
		/// <summary>
		/// Foreign key for Country.ID
		/// </summary>
		public int CountryId { get; set; }

		/// <summary>
		/// The Organisation object associated with this user
		/// </summary>
		public Organisation Organisation { get; set; }

		/// <summary>
		/// The Id of the organisation this user belongs to
		/// </summary>
		public int OrganisationId { get; set; }

		/// <summary>
		/// Empty constructor for User
		/// </summary>
		public User() { }

		/// <summary>
		/// Constructor for User
		/// </summary>
		/// <param name="username"></param>
		/// <param name="name"></param>
		/// <param name="email"></param>
		/// <param name="country"></param>
		/// <param name="date_of_birth"></param>
		public User(string username, string name, string email, int organisationId,
					int countryId, DateTime? date_of_birth)
		{
			this.Username = username;
			this.Email = email;
			this.OrganisationId = organisationId;
			this.Name = name;
			this.CountryId = countryId;
			this.DateOfBirth = date_of_birth;
		}
	}
}
