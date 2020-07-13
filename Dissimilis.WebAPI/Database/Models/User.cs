using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Dissimilis.WebAPI.Database.Models
{
	/// <summary>
	/// User entity that contains: 
	/// Id, email, name, DOB, org and country
	/// </summary>
	public class User : BaseEntity
	{
		/// <summary>
		/// The Id of the user
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// The microsoft Id of this user
		/// </summary>
		public Guid MsId { get; set; }
		
		//Commented out because testing outsourcing to Google
/*		/// <summary>
		/// User name of logged in user
		/// </summary>
		[Required]
		public string Username { get; set; }
		public string Password { get; set; }*/

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
		/// <param name="organisationId"></param>
		/// <param name="countryId"></param>
		/// <param name="date_of_birth"></param>
		public User(string name, string email, int organisationId,
					int countryId)
		{
			this.Email = email;
			this.OrganisationId = organisationId;
			this.Name = name;
			this.CountryId = countryId;
		}
	}
}
