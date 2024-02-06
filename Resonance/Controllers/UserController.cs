using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities;
using ResoClassAPI.Utilities.Interfaces;

namespace ResoClassAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ILogger<UserController> logger;
        private readonly IExcelReader excelReader;

        public UserController(IUserService _userService, ILogger<UserController> _logger, IExcelReader _excelReader)
        {
            userService = _userService;
            logger = _logger;
            excelReader = _excelReader;
        }

        [HttpPost("Upload")]
        public IActionResult UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file.");

            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            if (extension != ".xlsx")
                return BadRequest("Invalid file type.");

            if (excelReader.BulkUpload(file, SqlTableName.User))
                return Ok("Data uploaded successfully");
            else
                return BadRequest("Some Error occured!. Please check the format and try again");
        }

        [HttpGet]
        public async Task<ResponseDto> Get(int userId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("GetUser is called");
                var user = await userService.GetUser(userId);

                if (user != null)
                {
                    responseDto.Result = user;
                    responseDto.IsSuccess = true;
                }
                else
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Not Found";
                }
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }

        [HttpPost]
        public async Task<ResponseDto> Post(UserDto requestDto)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                if (requestDto == null)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Invalid Request";
                    return responseDto;
                }

                if (await userService.CreateUser(requestDto))
                {
                    responseDto.Result = requestDto;
                    responseDto.IsSuccess = true;
                }
                else
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Internal Server Error";
                }
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }

        [HttpPut("{id}")]
        public async Task<ResponseDto> Put(int id, UserDto requestDto)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                if (requestDto == null)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Invalid Request";
                    return responseDto;
                }

                if (await userService.UpdateUser(requestDto))
                {
                    responseDto.Result = requestDto;
                    responseDto.IsSuccess = true;
                }
                else
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Internal Server Error";
                }
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }

        [HttpDelete("{id}")]
        public async Task<ResponseDto> DeleteCallVolume(int id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                bool result = await userService.DeleteUser(id);

                if (!result)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Not Found";
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
