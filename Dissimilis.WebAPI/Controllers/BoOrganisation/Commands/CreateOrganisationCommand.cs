using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoOrganisation.Commands
{
    public class CreateOrganisationCommand : IRequest<UpdatedOrganisationCommandDto>
    {
        public CreateOrganisationDto Command { get; }

        public CreateOrganisationCommand(CreateOrganisationDto command)
        {
            Command = command;
        }
    }

    public class CreateOrganisationCommandHandler : IRequestHandler<CreateOrganisationCommand, UpdatedOrganisationCommandDto>
    {
        private readonly OrganisationRepository _OrganisationRepository;
        private readonly IAuthService _authService;

        public CreateOrganisationCommandHandler(OrganisationRepository OrganisationRepository, IAuthService authService)
        {
            _OrganisationRepository = OrganisationRepository;
            _authService = authService;
        }

        public async Task<UpdatedOrganisationCommandDto> Handle(CreateOrganisationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _authService.GetVerifiedCurrentUser();

            return new UpdatedOrganisationCommandDto(Organisation);
        }
    }
}
