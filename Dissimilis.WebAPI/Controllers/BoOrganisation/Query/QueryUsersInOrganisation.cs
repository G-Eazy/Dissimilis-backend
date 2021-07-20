using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoOrganisation;
using System.Linq;

namespace Dissimilis.WebAPI.Controllers.Bousers.Query
{
    public class QueryUsersInOrganisation : IRequest<UserDto[]>{ 
        
        public int OrganisationId { get; set; }
        public QueryUsersInOrganisation(int organisationId)
        {
            OrganisationId = organisationId;
        }
    }

    public class QueryUsersInOrganisationHandler : IRequestHandler<QueryUsersInOrganisation, UserDto[]>
    {
        private readonly OrganisationRepository _organisationRepository;

        public QueryUsersInOrganisationHandler(OrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<UserDto[]> Handle(QueryUsersInOrganisation request, CancellationToken cancellationToken)
        {
            var Organisation = await _organisationRepository.GetOrganisationById(request.OrganisationId, cancellationToken);
            var users = Organisation.Users
                .Select(gu => new UserDto(gu.User))
                .ToArray();

            return users;
        }
    }
}
