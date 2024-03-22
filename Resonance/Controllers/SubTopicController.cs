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
    public class SubTopicController : Controller
    {
        private readonly ISubTopicService subtopicService;
        private readonly ILogger<SubTopicController> logger;
        private readonly IExcelReader excelReader;
        private readonly IAwsHandler awsHandler;

        public SubTopicController(ISubTopicService _subtopicService, ILogger<SubTopicController> _logger, IExcelReader _excelReader,
            IAwsHandler _awsHandler)
        {
            subtopicService = _subtopicService;
            logger = _logger;
            excelReader = _excelReader;
            awsHandler = _awsHandler;
        }

        #region Admin
        
        [HttpGet]
        [Route("api/SubTopic/Get")]
        public async Task<ResponseDto> Get(int Id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetTopic");
                var chapter = await subtopicService.GetSubTopic(Id);

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
        [Route("api/SubTopic/GetAll")]
        public async Task<ResponseDto> GetAll()
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetAllSubTopics");
                var topic = await subtopicService.GetAllSubTopics();

                if (topic != null)
                {
                    responseDto.Result = topic;
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
        [Route("api/SubTopic/Create")]
        public async Task<ResponseDto> Post([ModelBinder(BinderType = typeof(JsonModelBinder))] SubTopicDto requestDto, IFormFile thumbnail)
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
                        string thumbnailUrl = await awsHandler.UploadImage(stream.ToArray(), "subtopics", thumbnail.FileName);
                        requestDto.Thumbnail = thumbnailUrl;
                    }
                }

                long newId = await subtopicService.CreateSubTopic(requestDto);
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
        [Route("api/SubTopic/Upload")]
        public async Task<ResponseDto> UploadExcel(IFormFile file)
        {
            ResponseDto responseDto = new ResponseDto();
            bool isDataUploaded = false;
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

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    List<SubTopicExcelRequestDto> subTopics = await excelReader.ReadSubTopicsFromExcel(stream);
                    isDataUploaded = await subtopicService.InsertSubTopics(subTopics);
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
        [Route("api/SubTopic/Update/{id}")]
        public async Task<ResponseDto> Put(long id, SubTopicDto requestDto)
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

                if (await subtopicService.UpdateSubTopic(requestDto))
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
        [Route("api/SubTopic/UpdateWithFile/{id}")]
        public async Task<ResponseDto> Put([ModelBinder(BinderType = typeof(JsonModelBinder))] SubTopicDto requestDto, IFormFile thumbnail)
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
                        string thumbnailUrl = await awsHandler.UploadImage(stream.ToArray(), "subtopics", thumbnail.FileName);
                        requestDto.Thumbnail = thumbnailUrl;
                    }
                }

                if (await subtopicService.UpdateSubTopic(requestDto))
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
        [Route("api/SubTopic/Delete/{id}")]
        public async Task<ResponseDto> Delete(long id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                bool result = await subtopicService.DeleteSubTopic(id);

                if (!result)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Not Found";
                }
                else
                {
                    responseDto.Result = "SubTopic Deleted Successfully";
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
        [Route("api/SubTopic/GetSubTopicsByTopicId")]
        public async Task<ResponseDto> GetSubTopicsByTopicId(long topicId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetSubTopicsByTopicId");
                var subTopics = await subtopicService.GetByTopicId(topicId);

                if (subTopics != null && subTopics.Count > 0)
                {
                    responseDto.Result = subTopics;
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
        [Route("api/SubTopic/GetSubTopicById")]
        public async Task<ResponseDto> GetSubTopicById(long Id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetSubTopicById");
                var chapter = await subtopicService.GetSubTopic(Id);

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
        [Route("api/SubTopic/GetVideosByChapterId")]
        public async Task<ResponseDto> GetVideosByChapterId(long chapterId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetVideosByChapterId");
                var videos = await subtopicService.GetVideosWithChapterId(chapterId);

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
        [Route("api/SubTopic/GetVideosByTopicId")]
        public async Task<ResponseDto> GetVideosByTopicId(long topicId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetVideosByTopicId");
                var videos = await subtopicService.GetVideosWithTopicId(topicId);

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
