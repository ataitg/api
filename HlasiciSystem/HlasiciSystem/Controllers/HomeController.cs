using Microsoft.AspNetCore.Mvc;

namespace HlasiciSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
