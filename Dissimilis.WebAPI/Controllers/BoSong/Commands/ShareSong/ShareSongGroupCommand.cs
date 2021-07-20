using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Controllers.BoGroup;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser;
using Dissimilis.WebAPI.Extensions;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers.BoSong.ShareSong
{
    public class ShareSongGroupCommand : IRequest<UpdatedSongCommandDto>
    {
        public int SongId { get; }

        public int GroupId { get; }

        public ShareSongGroupCommand(int songId, int groupId)
        {
            SongId = songId;
            GroupId = groupId;
        }
    }

    public class ShareSongGroupCommandHandler : IRequestHandler<ShareSongGroupCommand, UpdatedSongCommandDto>
    {
        private readonly SongRepository _songRepository;
        private readonly OrganisationRepository _groupRepository;
        private readonly IAuthService _IAuthService;

        public ShareSongGroupCommandHandler(SongRepository songRepository, OrganisationRepository groupRepository, IAuthService IAuthService)
        {
            _songRepository = songRepository;
            _groupRepository = groupRepository;
            _IAuthService = IAuthService;
        }

        public async Task<UpdatedSongCommandDto> Handle(ShareSongGroupCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _songRepository.Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongByIdForUpdate(request.SongId, cancellationToken);
            if (song.ArrangerId != currentUser.Id && !currentUser.IsSystemAdmin)
            {
                throw new UnauthorizedAccessException("You dont have permission to edit this song");
            }
            var groupToAdd = await _groupRepository.GetGroupById(request.GroupId, cancellationToken);
            var isShared = await _songRepository.GetSongSharedGroup(song.Id, groupToAdd.Id);
            if (!currentUser.GetAllGroupIds().Contains(groupToAdd.Id) && !currentUser.IsSystemAdmin)
            {
                throw new Exception("Can only tag a song with groups you are in");
            }
            if (isShared != null)
            {
                throw new Exception("Group already added to song");
            }

            var songSharedGroup = new SongSharedGroup()
            {
                GroupId = groupToAdd.Id,
                SongId = song.Id
            };
            groupToAdd.SharedSongs.Add(songSharedGroup);
            song.SharedGroups.Add(songSharedGroup);
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