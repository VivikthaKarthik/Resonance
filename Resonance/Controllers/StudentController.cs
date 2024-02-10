using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResoClassAPI.DTOs;
using ResoClassAPI.Services;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities.Interfaces;

namespace ResoClassAPI.Controllers
{
    //[Authorize]
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

        [HttpGet]
        [Route("GetRecommendedChaptersWithCourseId")]
        public async Task<ResponseDto> GetRecommendedChaptersWithCourseId(long courseId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetChaptersWithCourseId");
                var users = await studentService.GetRecommendedChaptersWithCourseId(courseId);

                if (users != null)
                {
                    responseDto.Result = users;
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


        [HttpGet]
        [Route("GetSubjectsWithCourseId")]
        public async Task<ResponseDto> GetSubjectsWithCourseId(long courseId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetUser");
                var subjects = await studentService.GetSubjectsWithCourseId(courseId);

                if (subjects != null && subjects.Count > 0)
                {
                    responseDto.Result = subjects;
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

        [HttpGet]
        [Route("GetChaptersWithSubjectId")]
        public async Task<ResponseDto> GetChaptersWithSubjectId(long subjectId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetChaptersWithCourseId");
                var users = await studentService.GetChaptersWithSubjectId(subjectId);

                if (users != null)
                {
                    responseDto.Result = users;
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

        [HttpGet]
        [Route("GetVideosWithChapterId")]
        public async Task<ResponseDto> GetVideosWithChapterId(long chapterId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetVideosWithChapterId");
                var videos = await studentService.GetVideosWithChapterId(chapterId);

                if (videos != null)
                {
                    responseDto.Result = videos;
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


        [HttpGet]
        [Route("GetVideosWithTopicId")]
        public async Task<ResponseDto> GetVideosWithTopicId(long topicId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetVideosWithTopicId");
                var videos = await studentService.GetVideosWithTopicId(topicId);

                if (videos != null)
                {
                    responseDto.Result = videos;
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


        [HttpGet]
        [Route("GetVideosWithSubTopicId")]
        public async Task<ResponseDto> GetVideosWithSubTopicId(long subTopicId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetVideosWithSubTopicId");
                var videos = await studentService.GetVideosWithSubTopicId(subTopicId);

                if (videos != null)
                {
                    responseDto.Result = videos;
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
        [HttpGet]
        [Route("GetTopicsWithChapterId")]
        public async Task<ResponseDto> GetTopicsWithChapterId(long chapterId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetTopicsWithChapterId");
                var videos = await studentService.GetTopicsWithChapterId(chapterId);

                if (videos != null)
                {
                    responseDto.Result = videos;
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

        #endregion

    }
}
