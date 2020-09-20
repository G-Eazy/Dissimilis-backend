using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dissimilis.WebAPI.DTOs;
using Dissimilis.DbContext;
using Dissimilis.WebAPI.Repositories;
using Experis.Ciber.Web.API.Controllers;

namespace Dissimilis.WebAPI.Controllers
{
    [Route("api/voice")]
    [ApiController]
    public class PartController : UserControllerBase
    {
        private PartRepository repository;
        
        public PartController(DissimilisDbContext context)
        {
            this.repository = new PartRepository(context);
        }

        
        /// <summary>
        /// Create voice
        /// </summary>
        /// <param name="NewPartObject"></param>
        /// <returns>201</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePart([FromBody] NewPartDTO NewPartObject)
        {
            var result = await repository.CreatePart(NewPartObject, base.UserID);

            if (result != 0)
                return base.Created($"api/voice/{result}", $"{result}"); 
            else
                return base.BadRequest("Unable to create voice");
        }

        /// <summary>
        /// Get voice
        /// </summary>
        /// <returns>200</returns> 
        [HttpGet("{voiceId:int:min(1)}")]
        public async Task<IActionResult> GetPart(int voiceId)
        {
            var PartObject = await repository.GetPart(voiceId);
            if (PartObject != null)
                return base.Ok(PartObject);
            else
                return base.NotFound();
        }

        /// <summary>
        /// Update voice
        /// </summary>
        /// <param name="UpdatePartObject"></param>
        /// <returns>204</returns>
        [HttpPatch]
        public async Task<IActionResult> UpdatePart([FromBody] UpdatePartDTO UpdatePartObject)
        {
            var result = await repository.UpdatePart(UpdatePartObject, base.UserID);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("Unable to update Voice");
        }
        
        /// <summary>
        /// Delete voice
        /// </summary>
        /// <returns>204</returns> 
        [HttpDelete("{voiceId:int:min(1)}")]
        public async Task<IActionResult> DeletePart(int voiceId)
        {
            bool result = await repository.DeletePart(voiceId, base.UserID);
            if (result)
                return base.NoContent();
            else
                return base.BadRequest("Unable to delete Voice");
        }

        
        
    }
}
