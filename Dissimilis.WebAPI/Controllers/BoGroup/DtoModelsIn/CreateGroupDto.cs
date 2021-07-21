namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn
{
    public class CreateGroupDto
    {
        public string Name { get; set; }
        public int OrganisationId { get; set; }
        public int FirstAdminId { get; set; }
    }
}
