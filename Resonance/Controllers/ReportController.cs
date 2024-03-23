using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResoClassAPI.DTOs;
using ResoClassAPI.Services;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities.Interfaces;

namespace ResoClassAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> logger;
        private readonly IReportService reportService;

        public ReportController(IReportService _reportService, ILogger<ReportController> _logger)
        {
            reportService = _reportService;
            logger = _logger;
        }

        [HttpGet]
        [Route("api/Report/GetSubjectsReport")]
        public async Task<ResponseDto> GetSubjectsReport()
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetSubjectsReport");
                var report = await reportService.GetSubjectsReport();

                if (report != null)
                {
                    responseDto.Result = report;
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

        //[HttpGet]
        //[Route("api/Report/GetStudentReport")]
        //public async Task<ResponseDto> GetStudentReport()
        //{
        //    ResponseDto responseDto = new ResponseDto();
        //    try
        //    {
        //        logger.LogInformation("Requested GetStudentReport");
        //        var report = await reportService.GetStudentReport();

        //        if (report != null)
        //        {
        //            responseDto.Result = report;
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

        [HttpGet]
        [Route("api/Report/GetSubjectReport")]
        public async Task<ResponseDto> GetSubjectReport(long id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetSubjectReport");
                var report = await reportService.GetSubjectReport(id);

                if (report != null)
                {
                    responseDto.Result = report;
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

        //[HttpGet]
        //[Route("api/Report/GetChapterReport")]
        //public async Task<ResponseDto> GetChapterReport(long id)
        //{
        //    ResponseDto responseDto = new ResponseDto();
        //    try
        //    {
        //        logger.LogInformation("Requested GetChapterReport");
        //        var report = await reportService.GetChapterReport(id);

        //        if (report != null)
        //        {
        //            responseDto.Result = report;
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

        [HttpGet]
        [Route("api/Report/GetAssessmentReport")]
        public async Task<ResponseDto> GetAssessmentReport(long id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetAssessmentConfig");
                var report = await reportService.GetAssessmentReport(id);

                if (report != null)
                {
                    responseDto.Result = report;
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
