using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoGroup;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.Bogroup.Query
{
    public class QueryGroupById : IRequest<GroupByIdDto>
    {
        public int GroupId { get; }

        public QueryGroupById(int groupId)
        {
            GroupId = groupId;
        }
    }

    public class QuerygroupByIdHandler : IRequestHandler<QueryGroupById, GroupByIdDto>
    {
        private readonly GroupRepository _groupRepository;

        public QuerygroupByIdHandler(GroupRepository repository)
        {
            _groupRepository = repository;
        }

        public async Task<GroupByIdDto> Handle(QueryGroupById request, CancellationToken cancellationToken)
        {
            var result = await _groupRepository.GetGroupById(request.groupId, cancellationToken);

            return new GroupByIdDto(result);
        }
    }
}
