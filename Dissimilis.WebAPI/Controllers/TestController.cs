using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dissimilis.WebAPI.Controllers
{
    [Route("api/oliver")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("{name}")]
        public ActionResult<string> MyMethod(string name, int bob)
        {

            //200.ok
            return base.Ok("hello, this is " + name + " your integer is " + bob);
        }
    }
}
