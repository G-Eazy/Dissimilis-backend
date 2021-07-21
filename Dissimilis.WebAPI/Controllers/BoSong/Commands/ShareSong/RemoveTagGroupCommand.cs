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
    public class RemoveTagGroupCommand : IRequest<SongTagGroupDto>
    {
        public int SongId { get; }

        public int GroupId { get; }

        public RemoveTagGroupCommand(int songId, int groupId)
        {
            SongId = songId;
            GroupId = groupId;
        }
    }

    public class RemoveTagGroupCommandHandler : IRequestHandler<RemoveTagGroupCommand, SongTagGroupDto>
    {
        private readonly SongRepository _songRepository;
        private readonly OrganisationRepository _groupRepository;
        private readonly IAuthService _IAuthService;

        public RemoveTagGroupCommandHandler(SongRepository songRepository, OrganisationRepository groupRepository, IAuthService IAuthService)
        {
            _songRepository = songRepository;
            _groupRepository = groupRepository;
            _IAuthService = IAuthService;
        }

        public async Task<SongTagGroupDto> Handle(RemoveTagGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongByIdForUpdate(request.SongId, cancellationToken);
            if (song.ArrangerId != currentUser.Id && !currentUser.IsSystemAdmin)
            {
                throw new UnauthorizedAccessException("You dont have permission to edit this song");
            }
            var groupToRemove = await _groupRepository.GetGroupById(request.GroupId, cancellationToken);
            var GroupTag = await _songRepository.GetSongSharedGroup(song.Id, groupToRemove.Id);
            if (!currentUser.GetAllGroupIds().Contains(groupToRemove.Id) && !currentUser.IsSystemAdmin)
            {
                throw new Exception("You need to be in the group you want to remove");
            }
            if (GroupTag == null)
            {
                throw new Exception($"Song not tagged with group {request.GroupId}");
            }

            groupToRemove.SharedSongs.Remove(GroupTag);
            song.SharedGroups.Remove(GroupTag);
            await _songRepository.DeleteGroupTag(GroupTag, cancellationToken);

            await _songRepository.UpdateAsync(cancellationToken);
            return new SongTagGroupDto(song);
        }
    }
}