using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoGroup;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser;
using Dissimilis.WebAPI.Services;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands.AddTags
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

    public class UpdateTagGroupCommandHandler : IRequestHandler<UpdateTagGroupCommand, ShortGroupDto[]>
    {
        private readonly SongRepository _songRepository;
        private readonly UserRepository _userRepository;
        private readonly GroupRepository _groupRepository;
        private readonly IAuthService _IAuthService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public UpdateTagGroupCommandHandler(SongRepository songRepository, UserRepository userRepository, GroupRepository groupRepository, IAuthService IAuthService, IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _IAuthService = IAuthService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<ShortGroupDto[]> Handle(UpdateTagGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _IAuthService.GetVerifiedCurrentUser();
            var song = await _songRepository.GetSongWithTagsSharedUsers(request.SongId, cancellationToken);

            if (!await _IPermissionCheckerService.CheckPermission(song, currentUser, Operation.Modify, cancellationToken)) throw new UnauthorizedAccessException( "You dont have permission to edit this song");

            var groupIds = await _userRepository.GetGroupUserIds(currentUser);

            if (!request.GroupIds.All(groupIds.Contains) && !currentUser.IsSystemAdmin)
            {
                throw new UnauthorizedAccessException("You need to be in the group you want to tag your song with.");
            }
            foreach (var groupId in request.GroupIds)
            {
                var groupToUpdate = await _groupRepository.GetGroupByIdAsync(groupId, cancellationToken);

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