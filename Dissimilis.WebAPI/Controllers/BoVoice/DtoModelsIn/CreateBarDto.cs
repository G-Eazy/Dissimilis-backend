using Dissimilis.WebAPI.DTOs;

namespace Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn
{
    public class CreateBarDto
    {
        public int BarNumber { get; set; }
        public bool RepBefore { get; set; }
        public bool RepAfter { get; set; }
        public int? House { get; set; }

    }
}
