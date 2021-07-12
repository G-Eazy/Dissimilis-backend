using Experis.Ciber.Authentication.Microsoft;
using Experis.Ciber.Authentication.Microsoft.APIObjects;
using System.Threading.Tasks;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Repositories;

namespace Dissimilis.WebAPI.Controllers.BoUser
{
    public class UserHandlingRepository
    {
        private readonly DissimilisDbContext _context;
        private readonly UserRepository _userRepo;
        private readonly OrganisationRepository _orgRepo;
        private readonly CountryRepository _countryRepo;
        public UserHandlingRepository(DissimilisDbContext context)
        {
            _context = context;
            _userRepo = new UserRepository(context);
            _orgRepo = new OrganisationRepository(context);
            _countryRepo = new CountryRepository(context);
        }

        public async Task<User> CreateOrFindUser(UserEntityMetadata userMeta, MSGraphAPI graphApi)
        {
            var user = Task.Run(() => _userRepo.CreateOrFindUserAsync(userMeta)).Result;
            OrganizationMetadata orgData;
            try
            {
                orgData = await graphApi.GetOrganizationAsync();
            }
            catch
            {
                orgData = null;
            }

            if (user.CountryId == null)
            {
                //The create or find method handles null values
                var country = await this._countryRepo.CreateOrFindCountryAsync(orgData);
                user = await this._userRepo.UpdateUserCountryAsync(user, country);
            }

            return user;
        }
    }
}
