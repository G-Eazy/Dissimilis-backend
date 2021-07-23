using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoGroup.Query
{
    public class QueryGroupMemberByIds : IRequest<QueryMemberByIdsDto>
    {
        public int UserId { get; }
        public int GroupId { get; }

        public QueryGroupMemberByIds(int userId, int groupId)
        {
            UserId = userId;
            GroupId = groupId;
        }
    }

    public class QueryGroupMemberByIdsHandler : IRequestHandler<QueryGroupMemberByIds, QueryMemberByIdsDto>
    {
        private readonly GroupRepository _groupRepository;

        public QueryGroupMemberByIdsHandler(GroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public async Task<QueryMemberByIdsDto> Handle(QueryGroupMemberByIds request, CancellationToken cancellationToken)
        {
            var fetchedMember = await _groupRepository.GetGroupUserAsync(request.UserId, request.GroupId, cancellationToken);
            return new QueryMemberByIdsDto() { UserId = fetchedMember.UserId, GroupId = fetchedMember.GroupId };
        }
    }
}
