namespace Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn
{
    public class CreateGroupDto
    {
        public string Name { get; set; }
        public int OrganisationId { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Description { get; set; }
        public int FirstAdminId { get; set; }
    }
}
