using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using System.Collections.Generic;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class SongTagGroupDto
    {
        public ICollection<SongSharedGroup> GroupTags{ get; set; }

        public SongTagGroupDto(Song song)
        {
            GroupTags = song.SharedGroups;
        }
    }

    public class SongTagOrganisationDto
    {
        public ICollection<SongSharedOrganisation> OrganisationTags { get; set; }
        public SongTagOrganisationDto(Song song)
        {
            OrganisationTags = song.SharedOrganisations;
        }
    }


}
