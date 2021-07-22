using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.Boorganisation.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoOrganisation;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.Boorganisation.Query
{
    public class QueryOrganisationById : IRequest<OrganisationByIdDto>
    {
        public int OrganisationId { get; }

        public QueryOrganisationById(int organisationId)
        {
            OrganisationId = organisationId;
        }
    }

    public class QueryOrganisationByIdHandler : IRequestHandler<QueryOrganisationById, OrganisationByIdDto>
    {
        private readonly OrganisationRepository _organisationRepository;

        public QueryOrganisationByIdHandler(OrganisationRepository repository)
        {
            _organisationRepository = repository;
        }

        public async Task<OrganisationByIdDto> Handle(QueryOrganisationById request, CancellationToken cancellationToken)
        {
            var result = await _organisationRepository.GetOrganisationById(request.OrganisationId, cancellationToken);

            return new OrganisationByIdDto(result);
        }
    }
}
