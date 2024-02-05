using Microsoft.AspNetCore.Mvc;

namespace Resonance.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {

        [HttpGet]
        public string Get()
        {
            return "API is Running successfully";
        }
    }
}
