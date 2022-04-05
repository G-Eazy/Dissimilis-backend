using Dissimilis.DbContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class ShortOrganisationDto
    {
        public int OrganisationId { get; set; }
        public string GroupName { get; set; }

        public ShortOrganisationDto(Organisation organisation)
        {
            OrganisationId = organisation.Id;
            GroupName = organisation.Name;
        }
    }
}
