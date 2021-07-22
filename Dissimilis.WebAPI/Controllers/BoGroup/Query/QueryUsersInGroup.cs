using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoGroup;
using System.Linq;

namespace Dissimilis.WebAPI.Controllers.Bousers.Query
{
    public class QueryUsersInGroup : IRequest<UserDto[]>{ 
        
        public int GroupId { get; set; }
        public QueryUsersInGroup(int roupId)
        {
            GroupId = roupId;
        }
    }

    public class QueryUsersInGroupHandler : IRequestHandler<QueryUsersInGroup, UserDto[]>
    {
        private readonly GroupRepository _groupRepository;

        public QueryUsersInGroupHandler(GroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<UserDto[]> Handle(QueryUsersInGroup request, CancellationToken cancellationToken)
        {
            // Todo: Regulate access to commands pertaining to fetching of users and / or groups/orgs.
            var Group = await _groupRepository.GetGroupById(request.GroupId, cancellationToken);
            var users = Group.Users
                .Select(ou => new UserDto(ou.User))
                .ToArray();

            return users;
        }
    }
}
