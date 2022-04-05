using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class QueryDeletedMyLibary : IRequest<SongIndexDto[]> 
    { 

        public QueryDeletedMyLibary()
        {
        }
    }

    public class QueryDeletedMyLibaryHandler : IRequestHandler<QueryDeletedMyLibary, SongIndexDto[]>
    {
        private readonly SongRepository _repository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;


        public QueryDeletedMyLibaryHandler(SongRepository repository, IAuthService authService, IPermissionCheckerService IPermissionCheckerService) 
        {
            _repository = repository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;

        }

        public async Task<SongIndexDto[]> Handle(QueryDeletedMyLibary request, CancellationToken cancellationToken)
        {
            var user = _authService.GetVerifiedCurrentUser();

            var result = await _repository.GetMyDeletedSongs(user, cancellationToken);

            foreach(var song in result)
            {
                if (!await _IPermissionCheckerService.CheckPermission(song, user, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException();
            }
            
            return result.Select(s => new SongIndexDto(s, true)).ToArray();
        }
    }
}
