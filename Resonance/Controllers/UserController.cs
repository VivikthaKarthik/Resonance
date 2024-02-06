using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;

namespace ResoClassAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ResponseDto> Get(int userId)
        {
            ResponseDto _response = new ResponseDto();
            try
            {
                _logger.LogInformation("GetUser is called");
                var user = await _userService.GetUser(userId);

                if (user != null)
                {
                    _response.Result = user;
                    _response.IsSuccess = true;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Not Found";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        public async Task<ResponseDto> Post(UserDto requestDto)
        {
            ResponseDto _response = new ResponseDto();
            try
            {
                if (requestDto == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Invalid Request";
                    return _response;
                }

                if (await _userService.CreateUser(requestDto))
                {
                    _response.Result = requestDto;
                    _response.IsSuccess = true;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Internal Server Error";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut("{id}")]
        public async Task<ResponseDto> Put(int id, UserDto requestDto)
        {
            ResponseDto _response = new ResponseDto();
            try
            {
                if (requestDto == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Invalid Request";
                    return _response;
                }

                if (await _userService.UpdateUser(requestDto))
                {
                    _response.Result = requestDto;
                    _response.IsSuccess = true;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Internal Server Error";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete("{id}")]
        public async Task<ResponseDto> DeleteCallVolume(int id)
        {
            ResponseDto _response = new ResponseDto();
            try
            {
                bool result = await _userService.DeleteUser(id);

                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Not Found";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
