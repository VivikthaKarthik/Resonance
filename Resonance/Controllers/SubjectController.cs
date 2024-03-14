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
    public class SubjectController : Controller
    {
        private readonly ISubjectService subjectService;
        private readonly ILogger<SubjectController> logger;
        private readonly IExcelReader excelReader;
        private readonly IAwsHandler awsHandler;

        public SubjectController(ISubjectService _subjectService, ILogger<SubjectController> _logger, IExcelReader _excelReader,
            IAwsHandler _awsHandler)
        {
            subjectService = _subjectService;
            logger = _logger;
            excelReader = _excelReader;
            awsHandler = _awsHandler;
        }

        #region Admin

        [HttpGet]
        [Authorize(Policy = "Admin")]
        [Route("api/Subject/Get")]
        public async Task<ResponseDto> Get(int Id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetUser");
                var chapter = await subjectService.GetSubject(Id);

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
        [Authorize(Policy = "Admin")]
        [Route("api/Subject/GetAll")]
        public async Task<ResponseDto> GetAll()
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetAllSubjects");
                var users = await subjectService.GetAllSubjects();

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
        [Authorize(Policy = "Admin")]
        [Route("api/Subject/Create")]
        public async Task<ResponseDto> Post([ModelBinder(BinderType = typeof(JsonModelBinder))] SubjectDto requestDto, IFormFile thumbnail)
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

                var extension = "." + thumbnail.FileName.Split('.')[thumbnail.FileName.Split('.').Length - 1];
                if (extension != ".png" && extension != ".jpg" && extension != ".webp")
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Invalid file type.";
                    return responseDto;
                }
                else
                {
                    using (var stream = new MemoryStream())
                    {
                        await thumbnail.CopyToAsync(stream);
                        stream.Position = 0;
                        string thumbnailUrl = await awsHandler.UploadImage(stream.ToArray(), "subjects", thumbnail.FileName);
                        requestDto.Thumbnail = thumbnailUrl;
                    }

                }
                long newId = await subjectService.CreateSubject(requestDto);
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
        [Authorize(Policy = "Admin")]
        [Route("api/Subject/Upload")]
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

                    List<SubjectDto> subjects = await excelReader.ReadSubjectsFromExcel(stream);
                    isDataUploaded = await subjectService.InsertSubjectsAndLinkToCourses(subjects);
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

        [HttpPut]
        [Authorize(Policy = "Admin")]
        [Route("api/Subject/Update/{id}")]
        public async Task<ResponseDto> Put(long id, SubjectDto requestDto)
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

                if (await subjectService.UpdateSubject(requestDto))
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

        [HttpPut]
        [Authorize(Policy = "Admin")]
        [Route("api/Subject/UpdateWithFile/{id}")]
        public async Task<ResponseDto> Put(long id, [ModelBinder(BinderType = typeof(JsonModelBinder))] SubjectDto requestDto, IFormFile thumbnail)
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



                var extension = "." + thumbnail.FileName.Split('.')[thumbnail.FileName.Split('.').Length - 1];
                if (extension != ".png" && extension != ".jpg" && extension != ".webp")
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Invalid file type.";
                    return responseDto;
                }
                else
                {
                    using (var stream = new MemoryStream())
                    {
                        await thumbnail.CopyToAsync(stream);
                        stream.Position = 0;
                        string thumbnailUrl = await awsHandler.UploadImage(stream.ToArray(), "subjects", thumbnail.FileName);
                        requestDto.Thumbnail = thumbnailUrl;
                    }
                }

                if (await subjectService.UpdateSubject(requestDto))
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
        [Authorize(Policy = "Admin")]
        [Route("api/Subject/Delete/{id}")]
        public async Task<ResponseDto> Delete(long id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                bool result = await subjectService.DeleteSubject(id);

                if (!result)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Not Found";
                }
                else
                {
                    responseDto.Result = "Subject Deleted Successfully";
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
        [Route("api/Subject/GetSubjectsByCourseId")]
        public async Task<ResponseDto> GetSubjectsByCourseId(long courseId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetSubjectsByCourseId");
                var subjects = await subjectService.GetSubjectsWithCourseId(courseId);

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

        #endregion

    }
}

