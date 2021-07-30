using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoOrganisation;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser;
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

    public class UpdateTagOrganisationCommandHandler : IRequestHandler<UpdateTagOrganisationCommand, ShortOrganisationDto[]>
    {
        private readonly SongRepository _songRepository;
        private readonly UserRepository _userRepository;
        private readonly OrganisationRepository _organisationRepository;
        private readonly IAuthService _IAuthService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public UpdateTagOrganisationCommandHandler(SongRepository songRepository, UserRepository userRepository, OrganisationRepository organisationRepository, IAuthService IAuthService, IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _userRepository = userRepository;
            _organisationRepository = organisationRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<ShortOrganisationDto[]> Handle(UpdateTagOrganisationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongWithTagsSharedUsers(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException("You dont have permission to edit this song");

            var organisationIds = await _userRepository.GetOrganisationUserIds(currentUser);

            if (!request.OrganisationIds.All(organisationIds.Contains) && !currentUser.IsSystemAdmin)
            {
                throw new UnauthorizedAccessException("You need to be in the organisation you want to tag your song with.");
            }
            foreach (var organisationId in request.OrganisationIds)
            {
                var organisationToUpdate = await _organisationRepository.GetOrganisationByIdAsync(organisationId, cancellationToken);

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