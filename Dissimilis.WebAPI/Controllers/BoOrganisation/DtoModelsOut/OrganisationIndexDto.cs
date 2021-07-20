using Dissimilis.DbContext.Models;
using System;
namespace Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsOut
{
    public class OrganisationIndexDto
    {
            public string OrganisationName { get; set; }
            public int OrganisationId { get; set; }
            public DateTimeOffset? CreatedOn { get; set; }

            public OrganisationIndexDto(Organisation organisation)
            {

                OrganisationId = organisation.Id;
                OrganisationName = organisation.Name;
                CreatedOn = organisation.CreatedOn;
            }
    }
}

