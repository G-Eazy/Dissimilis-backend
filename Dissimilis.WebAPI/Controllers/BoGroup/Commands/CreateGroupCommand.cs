using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoGroup.Commands
{
    public class CreateGroupCommand : IRequest<UpdatedGroupCommandDto>
    {
        public CreateGroupDto Command { get; }

        public CreateGroupCommand(CreateGroupDto command)
        {
            Command = command;
        }
    }

    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, UpdatedGroupCommandDto>
    {
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _authService;

        public CreateGroupCommandHandler(GroupRepository groupRepository, IAuthService authService)
        {
            _groupRepository = groupRepository;
            _authService = authService;
        }

        public async Task<UpdatedGroupCommandDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();

            return new UpdatedGroupCommandDto(group);
        }
    }
}