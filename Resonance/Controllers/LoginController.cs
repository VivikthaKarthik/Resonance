using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resonance.DTOs;
using Resonance.Services.Interfaces;

namespace Resonance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyPolicy")]
    public class LoginController : ControllerBase
    {
        private IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IAuthService authService, IMapper mapper, ILogger<LoginController> logger)
        {
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto user)
        {
            _logger.LogInformation("New Login Request");
            IActionResult response = Unauthorized("Invalid Credentials");

            if (user != null && !string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.Password))
            {
                var token = await _authService.AuthenticateUser(user);

                if (!string.IsNullOrEmpty(token))
                {
                    response = Ok(new { token = token });
                }
            }
            return response;
        }
    }
}
