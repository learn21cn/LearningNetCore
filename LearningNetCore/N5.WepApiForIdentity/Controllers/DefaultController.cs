using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace N5.WepApiForIdentity.Controllers
{
    [Route("api/[controller]")]
    public class DefaultController : Controller
    {
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var username = User.Claims.First(x => x.Type == "email").Value;
            return Ok(username);
        }
    }
}