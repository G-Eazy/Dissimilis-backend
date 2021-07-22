using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models;
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
            OrganisationTags = song.SharedOrganisations.Select(x => new ShortOrganisationDto(x.Organisation));
            GroupTags = song.SharedGroups.Select(x => new ShortGroupDto(x.Group));
            SharedWithUsers = song.SharedUsers.Select(x => new ShortUserDto(x.User));
        }
    }

    public class ShortOrganisationDto
    {
        public int OrganisationId { get; set; }
        public string Name { get; set; }

        public ShortOrganisationDto(Group group)
        {
            OrganisationId = group.Id;
            Name = group.Name;
        }
        public ShortOrganisationDto(Organisation organisation)
        {
            OrganisationId = organisation.Id;
            Name = organisation.Name;
        }
    }
    public class ShortGroupDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; }

        public ShortGroupDto(Group group)
        {
            GroupId = group.Id;
            Name = group.Name;
        }
    }

    public class ShortUserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ShortUserDto(User user)
        {
            UserId = user.Id;
            Name = user.Name;
            Email = user.Email;
        }
    }
}
