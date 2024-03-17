using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using ResoClassAPI.DTOs;
using ResoClassAPI.Middleware;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services;
using ResoClassAPI.Services.Interfaces;

namespace ResoClassAPI.Controllers
{
    [ApiController]
    [EnableCors("MyPolicy")]
    public class LoginController : ControllerBase
    {
        private IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginController> _logger;
        private readonly ILoggerService _loggerService;

        public LoginController(IAuthService authService, IMapper mapper, ILogger<LoginController> logger, ILoggerService loggerService)
        {
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
            _loggerService = loggerService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/User/Authenticate")]
        public async Task<ResponseDto> AuthenticateWebUser(WebLoginDto user)
        {

            ResponseDto responseDto = new ResponseDto();
            try
            {
                await _loggerService.Info(typeof(LoginController), "User Login Requested", JsonConvert.SerializeObject(user));
                _logger.LogInformation("Student Login Requested from Web");

                if (user != null && !string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.Password))
                {
                    var token = await _authService.AuthenticateWebUser(user);

                    if (!string.IsNullOrEmpty(token))
                    {
                        responseDto.Result = token;
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
        [Route("api/Student/Authenticate")]
        public async Task<ResponseDto> AuthenticateWebStudent(WebLoginDto student)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                await _loggerService.Info(typeof(LoginController), "Student Login Requested from Web", JsonConvert.SerializeObject(student));
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
        [Route("api/Student/Mobile/Authenticate")]
        public async Task<ResponseDto> AuthenticateMobileStudent(MobileLoginDto student)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                await _loggerService.Info(typeof(LoginController), "Student Login Requested from Mobile", JsonConvert.SerializeObject(student));
                _logger.LogInformation("Student Login Requested from Mobile");

                if (student == null || string.IsNullOrEmpty(student.UserName) || string.IsNullOrEmpty(student.Password))
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Invalid Credentials";
                    return responseDto;
                }
                if (student == null || string.IsNullOrEmpty(student.DeviceId))
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Invalid DeviceId";
                    return responseDto;
                }
                if (student == null || string.IsNullOrEmpty(student.FireBaseId))
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Invalid FireBaseId";
                    return responseDto;
                }

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
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }

    }
}
