using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/c/[controller]")]
    public class PlatformsController : ControllerBase
    {
      public PlatformsController()
      {
        
      }

      [HttpPost]
      public ActionResult TestInboundConnection()
      {
        Console.WriteLine("--> Inbound POST # Command Service");

        return Ok("Inbound test from Platform Controllers");
      }
    }
}