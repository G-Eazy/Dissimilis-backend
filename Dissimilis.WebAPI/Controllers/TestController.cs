using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.WebAPI.Controllers
{
    [Route("api/oliver")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private DissimilisDbContext context;
        public TestController(DissimilisDbContext context)
        {
            this.context = context;
        }

        [HttpGet("users")]
        public ActionResult<Database.Models.User[]> GetAllUsers()
        {
            return this.context.Users
                .Include(x => x.Country)
                .Include(x => x.UserGrp)
                .ToArray();
        }

        [HttpGet("{name}")]
        public ActionResult<string> MyMethod(string name, int bob)
        {
            //200.ok
            return base.Ok("hello, this is " + name + " your integer is " + bob);
        }
    }
}
