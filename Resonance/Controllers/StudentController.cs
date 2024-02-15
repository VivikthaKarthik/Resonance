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


        [HttpGet]
        [Route("api/Student/GetProfile")]
        public async Task<ResponseDto> Get()
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetUser");
                var chapter = await studentService.GetProfile();

                if (chapter != null)
                {
                    responseDto.Result = chapter;
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

        #region Student
        [HttpPut]
        [Route("api/Student/ChangePassword")]
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

        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Route("api/Student/Upload")]
        public async Task<ResponseDto> UploadExcel(IFormFile file)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                bool isDataUploaded = false;
                if (file == null || file.Length == 0)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Invalid file.";
                    return responseDto;
                }

                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                if (extension != ".xlsx")
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Invalid file type.";
                    return responseDto;
                }

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    List<StudentProfileDto> subjects = await excelReader.ReadStudentsFromExcel(stream);
                    isDataUploaded = await studentService.InsertStudents(subjects);
                }

                if (isDataUploaded)
                {
                    responseDto.Result = "Data uploaded successfully";
                    responseDto.IsSuccess = true;
                }
                else
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Some Error occured!. Please check the format and try again";
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
