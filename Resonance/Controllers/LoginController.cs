using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResoClassAPI.DTOs;
using ResoClassAPI.Services.Interfaces;

namespace ResoClassAPI.Controllers
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
        [Route("User/Authenticate")]
        public async Task<IActionResult> AuthenticateWebUser(WebLoginDto user)
        {
            _logger.LogInformation("New Login Request");
            IActionResult response = Unauthorized("Invalid Credentials");

            if (user != null && !string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.Password))
            {
                var token = await _authService.AuthenticateWebUser(user);

                if (!string.IsNullOrEmpty(token))
                {
                    response = Ok(new { token = token });
                }
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Student/Authenticate")]
        public async Task<IActionResult> AuthenticateWebStudent(WebLoginDto user)
        {
            _logger.LogInformation("New Login Request");
            IActionResult response = Unauthorized("Invalid Credentials");

            if (user != null && !string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.Password))
            {
                var token = await _authService.AuthenticateWebStudent(user);

                if (!string.IsNullOrEmpty(token))
                {
                    response = Ok(new { token = token });
                }
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Student/Mobile/Authenticate")]
        public async Task<IActionResult> AuthenticateMobileStudent(MobileLoginDto user)
        {
            _logger.LogInformation("New Login Request");
            IActionResult response = Unauthorized("Invalid Credentials");

            if (user != null && !string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.Password))
            {
                var token = await _authService.AuthenticateMobileStudent(user);

                if (!string.IsNullOrEmpty(token))
                {
                    response = Ok(new { token = token });
                }
            }
            return response;
        }
    }
}
