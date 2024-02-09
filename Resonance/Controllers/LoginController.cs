using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services;
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
        public async Task<ResponseDto> AuthenticateWebStudent(WebLoginDto student)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                _logger.LogInformation("Student Login Requested from Web");

                if (student != null && !string.IsNullOrEmpty(student.UserName) && !string.IsNullOrEmpty(student.Password))
                {
                    var studentDetails = await _authService.AuthenticateWebStudent(student);

                    if (studentDetails != null && !string.IsNullOrEmpty(studentDetails.Token))
                    {
                        responseDto.Result = studentDetails;
                        responseDto.IsSuccess = true;
                    }
                    else
                    {
                        responseDto.IsSuccess = false;
                        responseDto.Message = "Invalid Credentials";
                    }
                }
                else
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Invalid Credentials";
                }
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Student/Mobile/Authenticate")]
        public async Task<ResponseDto> AuthenticateMobileStudent(MobileLoginDto student)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                _logger.LogInformation("Student Login Requested from Mobile");

                if (student != null && !string.IsNullOrEmpty(student.UserName) && !string.IsNullOrEmpty(student.Password))
                {
                    var studentDetails = await _authService.AuthenticateMobileStudent(student);

                    if (studentDetails != null && !string.IsNullOrEmpty(studentDetails.Token))
                    {
                        responseDto.Result = studentDetails;
                        responseDto.IsSuccess = true;
                    }
                    else
                    {
                        responseDto.IsSuccess = false;
                        responseDto.Message = "Invalid Credentials";
                    }
                }
                else
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Invalid Credentials";
                }
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }
    }
}
