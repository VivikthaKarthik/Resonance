using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResoClassAPI.DTOs;
using ResoClassAPI.Services;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities.Interfaces;

namespace ResoClassAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService studentService;
        private readonly ILogger<StudentController> logger;
        private readonly IExcelReader excelReader;

        public StudentController(IStudentService _studentService, ILogger<StudentController> _logger, IExcelReader _excelReader)
        {
            studentService = _studentService;
            logger = _logger;
            excelReader = _excelReader;
        }


        #region Student
        [HttpPut]
        [Route("ChangePassword")]
        public async Task<ResponseDto> ChangePassword(string password)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Invalid Request";
                    return responseDto;
                }

                if (await studentService.ChangePassword(password))
                {
                    responseDto.Result = "Password Updated Successfully";
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


        #endregion

    }
}
