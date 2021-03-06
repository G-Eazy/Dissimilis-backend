using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.Query
{
    public class QueryGetOrganisations : IRequest<OrganisationIndexDto[]>
    {
        public string FilterBy { get; }
        public QueryGetOrganisations(string filterBy)
        {
            FilterBy = filterBy;
        }
    }

    public class QueryGetOrganisationsHandler : IRequestHandler<QueryGetOrganisations, OrganisationIndexDto[]>
    {
        private readonly IAuthService _authService;
        private readonly OrganisationRepository _repository;

        public QueryGetOrganisationsHandler(OrganisationRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<OrganisationIndexDto[]> Handle(QueryGetOrganisations request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();
            var result = await _repository.GetOrganisationsAsync(request.FilterBy, currentUser, cancellationToken);

            return result.Select(organisation => new OrganisationIndexDto(organisation)).ToArray();
        }
    }
}
