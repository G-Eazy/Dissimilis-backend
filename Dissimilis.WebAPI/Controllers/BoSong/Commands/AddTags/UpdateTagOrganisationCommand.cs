using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoOrganisation;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.ShareSong
{
    public class UpdateTagOrganisationCommand : IRequest<ShortOrganisationDto[]>
    {
        public int SongId { get; }

        public int[] OrganisationIds { get; }

        public UpdateTagOrganisationCommand(int songId, int[] organisationIds)
        {
            SongId = songId;
            OrganisationIds = organisationIds;
        }
    }

    public class RemoveTagOrganisationCommandHandler : IRequestHandler<UpdateTagOrganisationCommand, ShortOrganisationDto[]>
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

        public async Task<ShortOrganisationDto[]> Handle(UpdateTagOrganisationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongWithTagsSharedUsers(request.SongId, cancellationToken);
            if (song.ArrangerId != currentUser.Id && !currentUser.IsSystemAdmin)
            {
                throw new UnauthorizedAccessException("You dont have permission to edit this song");
            }
            if (!request.OrganisationIds.All(x => currentUser.GetAllOrganisationIds().Contains(x)) && !currentUser.IsSystemAdmin)
            {
                throw new Exception("You need to be in the organisation you want to remove");
            }
            foreach (var organisationId in request.OrganisationIds)
            {
                var organisationToUpdate = await _organisationRepository.GetOrganisationById(organisationId, cancellationToken);

                var OrganisationTag = await _songRepository.GetSongSharedOrganisation(song.Id, organisationToUpdate.Id);
                if (OrganisationTag == null)
                {
                    await _songRepository.CreateAndAddOrganisationTag(song, organisationToUpdate);
                }
            }
            await _songRepository.RemoveRedundantOrganisationTags(request.OrganisationIds, song, cancellationToken);
            await _songRepository.UpdateAsync(cancellationToken);
            return song.OrganisationTags.Select(x => new ShortOrganisationDto(x.Organisation)).ToArray();
        }
    }
}