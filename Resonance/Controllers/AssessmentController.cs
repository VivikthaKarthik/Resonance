using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Services;
using ResoClassAPI.Utilities.Interfaces;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon;
using ResoClassAPI.Middleware;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.Utilities;

namespace ResoClassAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class AssessmentController : ControllerBase
    {
        private readonly IWordReader wordReader;
        private readonly IAwsHandler awsHandler;
        private readonly ILogger<AssessmentController> logger;
        private readonly IAssessmentService assessmentService;
        private readonly ILoggerService _loggerService;

        public AssessmentController(ILoggerService loggerService, IAssessmentService _assessmentService, ILogger<AssessmentController> _logger, IAwsHandler _awsHandler,
            IWordReader _wordReader)
        {
            assessmentService = _assessmentService;
            logger = _logger;
            awsHandler = _awsHandler;
            wordReader = _wordReader;
            _loggerService = loggerService;
        }

        [HttpGet]
        [Route("api/Assessment/GetAssessmentConfig")]
        public async Task<ResponseDto> GetAssessmentConfig()
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetAssessmentConfig");
                var config = await assessmentService.GetAssessmentConfiguration();

                if (config != null)
                {
                    responseDto.Result = config;
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
        [Route("api/Assessment/GetAssessmentSessions")]
        public async Task<ResponseDto> GetAssessmentSessions()
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetAssessmentConfig");
                var sessions = await assessmentService.GetAssessmentSessions();

                if (sessions != null)
                {
                    responseDto.Result = sessions;
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
        [Route("api/QuestionBank/UploadQuestions")]
        public async Task<ResponseDto> UploadQuestions([ModelBinder(BinderType = typeof(JsonModelBinder))] QuestionsUploadRequestDto requestDto, IFormFile document)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                if (document == null || document.Length == 0)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "File is Empty";
                    return responseDto;
                }
                
                if(requestDto.CourseId <= 0 && requestDto.TopicId <= 0 && requestDto.SubTopicId <= 0)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Questions should be linked to atleast one master data(Chapter / Topic / SubTopic)";
                    return responseDto;
                }


                List <QuestionsDto> questions = await wordReader.ProcessDocument(document);
                string response = string.Empty;
                if (questions != null && questions.Count > 0)
                {
                    response = await assessmentService.InsertQuestions(questions, requestDto);
                }

                if (response == "Success")
                {
                    responseDto.Result = "Questions Successfully uploaded";
                    responseDto.IsSuccess = true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(response))
                    {
                        _loggerService.Error(typeof(AssessmentController), response, "", "");
                        responseDto.Message = response;
                    }
                    else
                        responseDto.Message = "Some Error occured!. Please check the format and try again";
                    responseDto.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Error(typeof(GlobalExceptionHandler), ex.Message, ex.StackTrace, ex.GetType().Name);
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }

        [HttpGet]
        [Route("api/Assessment/GetAssessmentLevels")]
        public async Task<ResponseDto> GetAssessmentLevels(string type, long id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var questions = await assessmentService.GetAssessmentLevels(type, id);

                if (questions != null)
                {
                    responseDto.Result = questions;
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
        [Route("api/Assessment/GetQuestionsByChapter")]
        public async Task<ResponseDto> GetQuestionsByChapter(long id, long levelId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetChapter");
                var questions = await assessmentService.GetQuestionsByChapter(id, levelId);

                if (questions != null)
                {
                    responseDto.Result = questions;
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
        [Route("api/Assessment/GetQuestionsByTopic")]
        public async Task<ResponseDto> GetQuestionsByTopic(long id, long levelId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetChapter");
                var questions = await assessmentService.GetQuestionsByTopic(id, levelId);

                if (questions != null)
                {
                    responseDto.Result = questions;
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
        [Route("api/Assessment/GetQuestionsBySubTopic")]
        public async Task<ResponseDto> GetQuestionsBySubTopic(long id, long levelId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetChapter");
                var questions = await assessmentService.GetQuestionsBySubTopic(id, levelId);

                if (questions != null)
                {
                    responseDto.Result = questions;
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


        //[HttpPost]
        //[Route("api/Assessment/GetQuestions")]
        //public async Task<ResponseDto> GetQuestions(QuestionRequestDto request)
        //{
        //    ResponseDto responseDto = new ResponseDto();
        //    try
        //    {
        //        logger.LogInformation("Requested GetChapter");
        //        var questions = await assessmentService.GetQuestions(request);

        //        if (questions != null)
        //        {
        //            responseDto.Result = questions;
        //            responseDto.IsSuccess = true;
        //        }
        //        else
        //        {
        //            responseDto.IsSuccess = false;
        //            responseDto.Message = "Not Found";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        responseDto.IsSuccess = false;
        //        responseDto.Message = ex.Message;
        //    }
        //    return responseDto;
        //}

        //[HttpPost]
        //[Route("api/Assessment/StartAssessment")]
        //public async Task<ResponseDto> StartAssessment(long assessmentId)
        //{
        //    ResponseDto responseDto = new ResponseDto();
        //    try
        //    {
        //        logger.LogInformation("Requested GetChapter");
        //        var isStarted = await assessmentService.StartAssessment(assessmentId);

        //        if (isStarted)
        //        {
        //            responseDto.Result = "Assessment Started Successfully";
        //            responseDto.IsSuccess = true;
        //        }
        //        else
        //        {
        //            responseDto.IsSuccess = false;
        //            responseDto.Message = "Not Found";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        responseDto.IsSuccess = false;
        //        responseDto.Message = ex.Message;
        //    }
        //    return responseDto;
        //}

        //[HttpPost]
        //[Route("api/Assessment/EndAssessment")]
        //public async Task<ResponseDto> EndAssessment(long assessmentId)
        //{
        //    ResponseDto responseDto = new ResponseDto();
        //    try
        //    {
        //        logger.LogInformation("Requested GetChapter");
        //        var isCompleted = await assessmentService.EndAssessment(assessmentId);

        //        if (isCompleted)
        //        {
        //            responseDto.Result = "Assessment Completed Successfully";
        //            responseDto.IsSuccess = true;
        //        }
        //        else
        //        {
        //            responseDto.IsSuccess = false;
        //            responseDto.Message = "Not Found";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        responseDto.IsSuccess = false;
        //        responseDto.Message = ex.Message;
        //    }
        //    return responseDto;
        //}

        [HttpPost]
        [Route("api/Assessment/UpdateQuestionStatus")]
        public async Task<ResponseDto> UpdateQuestionStatus(UpdateAssessmentStatusDto request)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetChapter");
                var isUpdated = await assessmentService.UpdateQuestionStatus(request);

                if (isUpdated)
                {
                    responseDto.Result = "Assessment Started Successfully";
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
        [Route("api/Assessment/DeleteQuestions")]
        public async Task<ResponseDto> DeleteQuestions(List<long> ids)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetChapter");
                var isCompleted = await assessmentService.DeleteQuestions(ids);

                if (isCompleted)
                {
                    responseDto.Result = "Question(s) Deleted Successfully";
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
        [Route("api/Assessment/GetAllQuestions")]
        public async Task<ResponseDto> GetAllQuestions(QuestionsUploadRequestDto request)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetChapter");
                var questions = await assessmentService.GetQuestions(request);

                if (questions != null)
                {
                    responseDto.Result = questions;
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
        [Route("api/Assessment/GetAssessmentsByStudentId")]
        public async Task<ResponseDto> GetAssessmentsByStudentId(long id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetAssessmentConfig");
                var sessions = await assessmentService.GetAssessmentsByStudentId(id);

                if (sessions != null)
                {
                    responseDto.Result = sessions;
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


    }

}
