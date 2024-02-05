using Microsoft.AspNetCore.Mvc;

namespace ResoClassAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {

        [HttpGet]
        public string Get()
        {
            return "ResoClass API is Running successfully\nVersion: 2";
        }
    }
}
