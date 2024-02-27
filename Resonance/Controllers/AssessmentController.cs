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
        //[Authorize(Policy = "Admin")]
        [Route("api/Assessment/UploadQuestions")]
        public async Task<ResponseDto> UploadQuestions(IFormFile document)
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

                if (questions != null && questions.Count > 0)
                {
                    await assessmentService.InsertQuestions(questions);
                    responseDto.Result = questions;
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
    }

}
