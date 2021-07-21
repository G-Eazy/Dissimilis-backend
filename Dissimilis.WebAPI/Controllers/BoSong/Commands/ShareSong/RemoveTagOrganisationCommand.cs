using System;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoOrganisation;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.ShareSong
{
    public class RemoveTagOrganisationCommand : IRequest<SongTagOrganisationDto>
    {
        public int SongId { get; }

        public int OrganisationId { get; }

        public RemoveTagOrganisationCommand(int songId, int organisationId)
        {
            SongId = songId;
            OrganisationId = organisationId;
        }
    }

    public class RemoveTagOrganisationCommandHandler : IRequestHandler<RemoveTagOrganisationCommand, SongTagOrganisationDto>
    {
        private readonly SongRepository _songRepository;
        private readonly OrganisationRepository _organisationRepository;
        private readonly IAuthService _IAuthService;

        public RemoveTagOrganisationCommandHandler(SongRepository songRepository, OrganisationRepository organisationRepository, IAuthService IAuthService)
        {
            _songRepository = songRepository;
            _organisationRepository = organisationRepository;
            _IAuthService = IAuthService;
        }

        public async Task<SongTagOrganisationDto> Handle(RemoveTagOrganisationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongByIdForUpdate(request.SongId, cancellationToken);
            if (song.ArrangerId != currentUser.Id && !currentUser.IsSystemAdmin)
            {
                throw new UnauthorizedAccessException("You dont have permission to edit this song");
            }
            var organisationToRemove = await _organisationRepository.GetOrganisationById(request.OrganisationId, cancellationToken);
            var OrganisationTag = await _songRepository.GetSongSharedOrganisation(song.Id, organisationToRemove.Id);
            if (!currentUser.GetAllOrganisationIds().Contains(organisationToRemove.Id) && !currentUser.IsSystemAdmin)
            {
                throw new Exception("You need to be in the organisation you want to remove");
            }
            if (OrganisationTag == null)
            {
                throw new Exception($"Song not tagged with organisation {request.OrganisationId}");
            }

            organisationToRemove.SharedSongs.Remove(OrganisationTag);
            song.SharedOrganisations.Remove(OrganisationTag);
            await _songRepository.DeleteOrganisationTag(OrganisationTag, cancellationToken);

            await _songRepository.UpdateAsync(cancellationToken);
            return new SongTagOrganisationDto(song);
        }
    }
}