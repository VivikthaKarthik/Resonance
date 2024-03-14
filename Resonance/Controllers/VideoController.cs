﻿using AutoMapper;
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
    public class VideoController : Controller
    {
        private readonly IVideoService videoService;
        private readonly ILogger<SubTopicController> logger;
        private readonly IExcelReader excelReader;
        private readonly IAwsHandler awsHandler;

        public VideoController(IVideoService _videoService, ILogger<SubTopicController> _logger, IExcelReader _excelReader,
            IAwsHandler _awsHandler)
        {
            videoService = _videoService;
            logger = _logger;
            excelReader = _excelReader;
            awsHandler = _awsHandler;
        }

        #region Admin

        [HttpGet]
        [Authorize(Policy = "Admin")]
        [Route("api/Video/Get")]
        public async Task<ResponseDto> Get(int Id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetTopic");
                var chapter = await videoService.GetVideo(Id);

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
        [Route("api/Video/GetAll")]
        public async Task<ResponseDto> GetAll()
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetAllSubTopics");
                var topic = await videoService.GetAllVideos();

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
        [Route("api/Video/Create")]
        public async Task<ResponseDto> Post([ModelBinder(BinderType = typeof(JsonModelBinder))] VideoDto requestDto, IFormFile thumbnail)
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
                        string thumbnailUrl = await awsHandler.UploadImage(stream.ToArray(), "videos", thumbnail.FileName);
                        requestDto.ThumbNail = thumbnailUrl;
                    }

                }
                long newId = await videoService.CreateVideo(requestDto);
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
        [Route("api/Video/Upload")]
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

                    List<VideoExcelRequestDto> videos = await excelReader.ReadVideosFromExcel(stream);
                    isDataUploaded = await videoService.InsertVideos(videos);
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
        [Route("api/Video/Update/{id}")]
        public async Task<ResponseDto> Put(int id, VideoDto requestDto)
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

                if (await videoService.UpdateVideo(requestDto))
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
        [Route("api/Video/UpdateWithFile/{id}")]
        public async Task<ResponseDto> Put([ModelBinder(BinderType = typeof(JsonModelBinder))] VideoDto requestDto, IFormFile thumbnail)
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
                        string thumbnailUrl = await awsHandler.UploadImage(stream.ToArray(), "videos", thumbnail.FileName);
                        requestDto.ThumbNail = thumbnailUrl;
                    }

                }

                if (await videoService.UpdateVideo(requestDto))
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
        [Route("api/Video/Delete/{id}")]
        public async Task<ResponseDto> Delete(int id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                bool result = await videoService.DeleteVideo(id);

                if (!result)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Not Found";
                }
                else
                {
                    responseDto.Result = "Video Deleted Successfully";
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
        [Route("api/Video/GetVideosByChapterId")]
        public async Task<ResponseDto> GetVideosByChapterId(long chapterId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetVideosByChapterId");
                var videos = await videoService.GetVideosWithChapterId(chapterId);

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
        [Route("api/Video/GetVideosByTopicId")]
        public async Task<ResponseDto> GetVideosByTopicId(long topicId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetVideosByTopicId");
                var videos = await videoService.GetVideosWithTopicId(topicId);

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
        [Route("api/Video/GetVideosBySubTopicId")]
        public async Task<ResponseDto> GetVideosBySubTopicId(long subTopicId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetVideosBySubTopicId");
                var videos = await videoService.GetVideosWithSubTopicId(subTopicId);

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
