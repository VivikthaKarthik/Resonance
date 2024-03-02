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

        public AssessmentController(IAssessmentService _assessmentService, ILogger<AssessmentController> _logger, IAwsHandler _awsHandler,
            IWordReader _wordReader)
        {
            assessmentService = _assessmentService;
            logger = _logger;
            awsHandler = _awsHandler;
            wordReader = _wordReader;
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Route("api/QuestionBank/UploadQuestions")]
        public async Task<ResponseDto> UploadQuestions(IFormFile document, string? chapter, string? topic, string? subTopic)
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
                
                if(string.IsNullOrEmpty(chapter) && string.IsNullOrEmpty(topic) && string.IsNullOrEmpty(subTopic))
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Questions should be linked to atleast one master data(Chapter / Topic / SubTopic)";
                    return responseDto;
                }


                List <QuestionsDto> questions = await wordReader.ProcessDocument(document);
                string response = string.Empty;
                if (questions != null && questions.Count > 0)
                {
                    response = await assessmentService.InsertQuestions(questions, chapter, topic, subTopic);
                }

                if (response == "Success")
                {
                    responseDto.Result = "Questions Successfully uploaded";
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

        [HttpPost]
        [Route("api/Assessment/GetQuestions")]
        public async Task<ResponseDto> GetQuestions(QuestionRequestDto request)
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
    }

}
