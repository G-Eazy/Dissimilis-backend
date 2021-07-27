using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.Query
{
     public class QueryOrganisationMemberByIds: IRequest<QueryMemberByIdsDto>
    
    
        {
            public int UserId { get; }
            public int OrganisationId { get; }

            public QueryOrganisationMemberByIds(int userId, int organisationId)
            {
                UserId = userId;
                OrganisationId = organisationId;
            }
        }

        public class QueryOrganisationMemberByIdsHandler : IRequestHandler<QueryOrganisationMemberByIds, QueryMemberByIdsDto>
        {
            private readonly OrganisationRepository _organisationRepository;

            public QueryOrganisationMemberByIdsHandler(OrganisationRepository organisationRepository)
            {
                _organisationRepository = organisationRepository;
            }
            public async Task<QueryMemberByIdsDto> Handle(QueryOrganisationMemberByIds request, CancellationToken cancellationToken)
            {
                var fetchedMember = await _organisationRepository.GetOrganisationUserAsync(request.UserId, request.OrganisationId, cancellationToken);
                return new QueryMemberByIdsDto() { UserId = fetchedMember.UserId, OrganisationId = fetchedMember.OrganisationId };
            }
        }
}
