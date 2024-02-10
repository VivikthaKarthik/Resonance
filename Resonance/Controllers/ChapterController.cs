
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities;
using ResoClassAPI.Utilities.Interfaces;
namespace ResoClassAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class ChapterController : Controller
    {
        private readonly IChapterService chapterService;
        private readonly ILogger<ChapterController> logger;
        private readonly IExcelReader excelReader;

        public ChapterController(IChapterService _chapterService, ILogger<ChapterController> _logger, IExcelReader _excelReader)
        {
            chapterService = _chapterService;
            logger = _logger;
            excelReader = _excelReader;
        }

        #region Admin

        [HttpGet]
        [Route("api/Admin/Chapter/Get")]
        public async Task<ResponseDto> Get(long chapterId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetUser");
                var chapter = await chapterService.GetChapter(chapterId);

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

        [HttpGet]
        [Route("api/Admin/Chapter/GetAll")]
        public async Task<ResponseDto> GetAll()
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetAllChapters");
                var users = await chapterService.GetAllChapters();

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

        [HttpPost]
        [Route("api/Admin/Chapter/Create")]
        public async Task<ResponseDto> Post(ChapterRequestDto requestDto)
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
                long newId = await chapterService.CreateChapter(requestDto);
                if (newId > 0)
                {
                    requestDto.Id = newId;
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

        [HttpPost]
        [Route("api/Admin/Chapter/Upload")]
        public async Task<ResponseDto> UploadExcel(IFormFile file)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
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

                if (await excelReader.BulkUpload(file, SqlTableName.Chapter))
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

        [HttpPut]
        [Route("api/Admin/Chapter/Update/{id}")]
        public async Task<ResponseDto> Put(long id, ChapterRequestDto requestDto)
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

                if (await chapterService.UpdateChapter(requestDto))
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

        [HttpDelete]
        [Route("api/Admin/Chapter/Delete/{id}")]
        public async Task<ResponseDto> Delete(long id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                bool result = await chapterService.DeleteChapter(id);

                if (!result)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Not Found";
                }
                else
                {
                    responseDto.Result = "Chapter Deleted Successfully";
                    responseDto.IsSuccess = true;
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

        #region Student


        [HttpGet]
        [Route("api/Student/Chapter/GetRecommendedByCourseId")]
        public async Task<ResponseDto> GetRecommendedByCourseId(long courseId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetRecommendedByCourseId");
                var users = await chapterService.GetRecommendedChaptersWithCourseId(courseId);

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
        [Route("api/Student/Chapter/GetBySubjectId")]
        public async Task<ResponseDto> GetBySubjectId(long subjectId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetBySubjectId");
                var users = await chapterService.GetChaptersWithSubjectId(subjectId);

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

        #endregion

    }
}
