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
        public string FilterByRole { get; }
        public int? OrganisationId { get; }

        public QueryGetOrganisations(string filterByRole)
        {
            FilterByRole = filterByRole;
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
            var result = await _repository.GetOrganisationsAsync(request.FilterByRole, currentUser, cancellationToken);

            return result.Select(organisation => new OrganisationIndexDto(organisation)).ToArray();
        }
    }
}
