using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Services;
using ResoClassAPI.Utilities;
using ResoClassAPI.Utilities.Interfaces;
using ResoClassAPI.DTOs;
using ResoClassAPI.Middleware;

namespace ResoClassAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class ScheduledExamController : ControllerBase
    {
        private readonly IWordReader wordReader;
        private readonly ILoggerService loggerService;
        private readonly IScheduledExamService scheduledExamService;

        public ScheduledExamController(ILoggerService _loggerService, IScheduledExamService _scheduledExamService,
            IWordReader _wordReader)
        {
            scheduledExamService = _scheduledExamService;
            wordReader = _wordReader;
            loggerService = _loggerService;
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Route("api/ScheduledExam/CreateExam")]
        public async Task<ResponseDto> CreateExam([ModelBinder(BinderType = typeof(JsonModelBinder))] ScheduledExamRequestDto request, IFormFile document)
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

                List<QuestionsDto> questions = await wordReader.ProcessDocument(document);
                string response = string.Empty;
                if (questions != null && questions.Count > 0)
                {
                    response = await scheduledExamService.InsertQuestions(questions, request);
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
                        loggerService.Error(typeof(AssessmentController), response, "", "");
                        responseDto.Message = response;
                    }
                    else
                        responseDto.Message = "Some Error occured!. Please check the format and try again";
                    responseDto.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                loggerService.Error(typeof(GlobalExceptionHandler), ex.Message, ex.StackTrace, ex.GetType().Name);
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        [Route("api/ScheduledExam/GetScheduledExams")]
        public async Task<ResponseDto> GetScheduledExams()
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var exams = await scheduledExamService.GetScheduledExams();

                if (exams != null)
                {
                    responseDto.Result = exams;
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
        [Route("api/ScheduledExam/GetScheduledExamQuestions")]
        public async Task<ResponseDto> GetScheduledExamQuestions(long id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var questions = await scheduledExamService.GetScheduledExamQuestions(id);

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
    }
}
