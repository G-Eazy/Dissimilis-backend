using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoOrganisation;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dissimilis.WebAPI.Extensions;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Models;

namespace Dissimilis.WebAPI.Controllers.BoSong.ShareSong
{
    public class ShareSongOrganisationCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }
        public int OrganisationId { get; }

        public ShareSongOrganisationCommand(int songId, int organisationId)
        {
            SongId = songId;
            OrganisationId = organisationId;
        }

    }

    public class ShareSongOrganisationCommandHandler : IRequestHandler<ShareSongOrganisationCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly OrganisationRepository _organisationRepository;
        private readonly IAuthService _IAuthService;

        public ShareSongOrganisationCommandHandler(SongRepository songRepository, OrganisationRepository organisationRepository, IAuthService IAuthService)
        {
            _songRepository = songRepository;
            _organisationRepository = organisationRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(ShareSongOrganisationCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _songRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongByIdForUpdate(request.SongId, cancellationToken);
            if(song.ArrangerId != currentUser.Id && !currentUser.IsSystemAdmin)
            {
                throw new UnauthorizedAccessException("You dont have permission to edit this song");
            }
                var organisationToAdd = await _organisationRepository.GetOrganisationById(request.OrganisationId, cancellationToken);
                var isShared = await _songRepository.GetSongSharedOrganisation(song.Id, organisationToAdd.Id);

                if (!currentUser.GetAllOrganisationIds().Contains(organisationToAdd.Id) && !currentUser.IsSystemAdmin)
                {
                    throw new Exception("Can only tag a song with organisations you are in");
                }

                if (isShared != null)
                {
                    throw new Exception("Organisation already added to song");
                }
                
                var songSharedOrganisation = new SongSharedOrganisation()
                {
                    OrganisationId = organisationToAdd.Id,
                    SongId = song.Id
                };
                organisationToAdd.SharedSongs.Add(songSharedOrganisation);
                song.SharedOrganisations.Add(songSharedOrganisation);
            try
            {
                await _songRepository.UpdateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return new UpdatedSongCommandDto(song);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }


        }
    }
}