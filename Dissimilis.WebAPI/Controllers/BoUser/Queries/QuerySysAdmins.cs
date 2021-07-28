using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoUser.Queries
{
    public class QuerySysAdmins : IRequest<UserDto[]> { }

    public class QuerySysAdminsHandler : IRequestHandler<QuerySysAdmins, UserDto[]>
    {
        private readonly UserRepository _repository;

        public QuerySysAdminsHandler(UserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDto[]> Handle(QuerySysAdmins request, CancellationToken cancellationToken)
        {
            var users = await _repository.GetAllUsers(cancellationToken);

            return users
                .Where(u => u.IsSystemAdmin)
                .Select(u => new UserDto(u))
                .ToArray();
        }
    }
}
