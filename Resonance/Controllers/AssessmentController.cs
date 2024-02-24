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

namespace ResoClassAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class AssessmentController : ControllerBase
    {
        private readonly IWordReader wordReader;
        private readonly ILogger<AssessmentController> logger;

        public AssessmentController(IChapterService _chapterService, ILogger<AssessmentController> _logger, IWordReader _wordReader)
        {
            //chapterService = _chapterService;
            logger = _logger;
            wordReader = _wordReader;
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Route("api/Assessment/uploadQuestions")]
        public async Task<ResponseDto> UploadDocument(IFormFile document)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                List<QuestionsDto> questions = await wordReader.ProcessDocument(document);

                if (questions != null && questions.Count > 0)
                {
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
