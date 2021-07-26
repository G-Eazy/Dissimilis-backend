using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models.Song;
using System.Collections.Generic;
using System.Linq;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class ProtectionLevelSharedWithAndTagsDto
    {
        public IEnumerable<ShortOrganisationDto> OrganisationTags { get; set; }
        public IEnumerable<ShortGroupDto> GroupTags { get; set; }
        public IEnumerable<ShortUserDto> SharedWithUsers { get; set; }
        public int SongId { get; set; }
        public string ProtectionLevel { get; set; }

        public ProtectionLevelSharedWithAndTagsDto(Song song)
        {
            SongId = song.Id;
            ProtectionLevel = song.ProtectionLevel.GetDescription();
            OrganisationTags = song.OrganisationTags.Select(x => new ShortOrganisationDto(x.Organisation));
            GroupTags = song.GroupTags.Select(x => new ShortGroupDto(x.Group));
            SharedWithUsers = song.SharedUsers.Select(x => new ShortUserDto(x.User));
        }
    }
}
