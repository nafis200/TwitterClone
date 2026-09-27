using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TwitterClone.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AController : ControllerBase
    {
        public AController() { }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World prayer time now");
        }
    }
}