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

namespace Dissimilis.WebAPI.Controllers.BoSong.ShareSong
{
    public class ShareSongOrganisationCommand : INotification
    {
        public int SongId { get; }
        public ShareSongDto Command { get; }


        public ShareSongOrganisationCommand(int songId, ShareSongDto command)
        {
            SongId = songId;
            Command = command;
        }
    }

    public class ShareSongOrganisationCommandHandler : INotificationHandler<ShareSongOrganisationCommand>
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

        public async Task Handle(ShareSongOrganisationCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _songRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongByIdForUpdate(request.SongId, cancellationToken);
            if(song.ArrangerId != currentUser.Id && !currentUser.IsSystemAdmin)
            {
                throw new UnauthorizedAccessException("You dont have permission to edit this song");
            }
            foreach( var organisation in request.Command.ShareSongIds)
            {
                var organisationToAdd = await _organisationRepository.GetOrganisationById(organisation, cancellationToken);
                var isShared = await _songRepository.GetSongSharedOrganisation(song.Id, organisationToAdd.Id);
                
                if(isShared != null)
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
            }
            try
            {
                await _songRepository.UpdateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }


        }
    }
}