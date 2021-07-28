using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoUser.Queries
{
    public class QueryUserById : IRequest<UserDto> 
    {
        public int UserId { get; }

        public QueryUserById(int userId)
        {
            UserId = userId;
        }
    }

    public class QueryUserByIdHandler : IRequestHandler<QueryUserById, UserDto>
    {
        private readonly UserRepository _repository;

        public QueryUserByIdHandler(UserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDto> Handle(QueryUserById request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetUserById(request.UserId, cancellationToken);

            return new UserDto(user);
        }
    }
}