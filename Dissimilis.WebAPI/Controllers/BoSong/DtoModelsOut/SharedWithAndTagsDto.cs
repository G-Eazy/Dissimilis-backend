using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using System.Collections.Generic;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class SharedWithAndTagsDto
    {
        public ICollection<SongSharedOrganisation> OrganisationTags { get; set; }
        public ICollection<SongSharedGroup> GroupTags { get; set; }
        public ICollection<SongSharedUser> SharedWithUsers { get; set; }

        public SharedWithAndTagsDto(Song song)
        {
            OrganisationTags = song.SharedOrganisations;
            GroupTags = song.SharedGroups;
            SharedWithUsers = song.SharedUsers;
        }
    }
}
