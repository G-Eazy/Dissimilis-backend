using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoUser.Queries
{
    public class QueryAll : IRequest<UserDto[]> { }

    public class QueryAllHandler : IRequestHandler<QueryAll, UserDto[]>
    {
        private readonly UserRepository _repository;

        public QueryAllHandler(UserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDto[]> Handle(QueryAll request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllUsers(cancellationToken);

            return result
                .Select(u => new UserDto(u))
                .ToArray();
        }
    }
}