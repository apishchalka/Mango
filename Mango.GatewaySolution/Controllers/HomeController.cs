using Microsoft.AspNetCore.Mvc;

namespace Mango.GatewaySolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("Ocelot gateway");
        }
    }
}
