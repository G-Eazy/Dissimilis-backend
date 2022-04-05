using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.PagedExtensions;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoUser.Queries
{
    public class QueryUsersInMyGroups : IRequest<UserPagedResultDto>
    {
        public readonly UsersInMyGroupsDto Dto;

        public QueryUsersInMyGroups(UsersInMyGroupsDto dto)
        {
            Dto = dto;
        }
    }

    public class QueryUsersInMyGroupsHandler : IRequestHandler<QueryUsersInMyGroups, UserPagedResultDto>
    {
        private readonly UserRepository _repository;
        private readonly IAuthService _authService;

        public QueryUsersInMyGroupsHandler(UserRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<UserPagedResultDto> Handle(QueryUsersInMyGroups request, CancellationToken cancellationToken)
        {
             var currentUser = _authService.GetVerifiedCurrentUser();

            var dbResult = await _repository.GetPagedUsersInMyGroups(request.Dto, currentUser, cancellationToken);

            return new UserPagedResultDto(dbResult, request.Dto);
        }
    }
}