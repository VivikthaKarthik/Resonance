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
            return "Resonance API is Running successfully";
        }
    }
}
