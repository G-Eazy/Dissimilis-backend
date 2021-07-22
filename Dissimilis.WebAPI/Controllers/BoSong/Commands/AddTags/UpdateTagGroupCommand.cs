using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoGroup;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.ShareSong
{
    public class UpdateTagGroupCommand : IRequest<ShortGroupDto[]>
    {
        public int SongId { get; }

        public int[] GroupIds { get; }

        public UpdateTagGroupCommand(int songId, int[] groupIds)
        {
            SongId = songId;
            GroupIds = groupIds;
        }
    }

    public class RemoveTagGroupCommandHandler : IRequestHandler<UpdateTagGroupCommand, ShortGroupDto[]>
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

        public async Task<ShortGroupDto[]> Handle(UpdateTagGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongWithTagsSharedUsers(request.SongId, cancellationToken);
            if (song.ArrangerId != currentUser.Id && !currentUser.IsSystemAdmin)
            {
                throw new UnauthorizedAccessException("You dont have permission to edit this song");
            }
            if (!request.GroupIds.All(x => currentUser.GetAllGroupIds().Contains(x)) && !currentUser.IsSystemAdmin)
            {
                throw new Exception("You need to be in the group you want to remove");
            }
            foreach (var groupId in request.GroupIds)
            {
                var groupToUpdate = await _groupRepository.GetGroupById(groupId, cancellationToken);

                var GroupTag = await _songRepository.GetSongSharedGroup(song.Id, groupToUpdate.Id);
                if(GroupTag == null)
                {
                    await _songRepository.CreateAndAddGroupTag(song, groupToUpdate);
                }
            }
            await _songRepository.RemoveRedundantGroupTags(request.GroupIds, song, cancellationToken);
            await _songRepository.UpdateAsync(cancellationToken);
            return song.GroupTags.Select(x => new ShortGroupDto(x.Group)).ToArray();
        }
    }
}