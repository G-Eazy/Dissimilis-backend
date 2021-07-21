using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using System.Collections.Generic;
using System.Linq;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class ProtectionLevelSharedWithAndTagsDto
    {
        public IEnumerable<ShortOrganisationOrGroupDto> OrganisationTags { get; set; }
        public IEnumerable<ShortOrganisationOrGroupDto> GroupTags { get; set; }
        public IEnumerable<ShortUserDto> SharedWithUsers { get; set; }
        public int SongId { get; set; }
        public string ProtectionLevel { get; set; }

        public ProtectionLevelSharedWithAndTagsDto(Song song)
        {
            OrganisationTags = song.SharedOrganisations.Select(x => new ShortOrganisationOrGroupDto(x.Organisation));
            GroupTags = song.SharedGroups.Select(x => new ShortOrganisationOrGroupDto(x.Group));
            SharedWithUsers = song.SharedUsers.Select(x => new ShortUserDto(x.User));
            SongId = song.Id;
            ProtectionLevel = song.ProtectionLevel.GetDescription();
        }
    }

    public class ShortOrganisationOrGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ShortOrganisationOrGroupDto(Group group)
        {
            Id = group.Id;
            Name = group.Name;
        }
        public ShortOrganisationOrGroupDto(Organisation organisation)
        {
            Id = organisation.Id;
            Name = organisation.Name;
        }
    }

    public class ShortUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ShortUserDto(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
        }
    }
}
