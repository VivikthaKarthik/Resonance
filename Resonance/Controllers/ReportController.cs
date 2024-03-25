using DocumentFormat.OpenXml.Office2010.Excel;
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
        [Route("api/Report/TrackYourProgressBySubject")]
        public async Task<ResponseDto> TrackYourProgressBySubject(long subjectId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var report = await reportService.TrackYourProgressBySubject(subjectId);

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

        [HttpGet]
        [Route("api/Report/TrackYourProgressByChapter")]
        public async Task<ResponseDto> TrackYourProgressByChapter(long chapterId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var report = await reportService.TrackYourProgressByChapter(chapterId);

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

        [HttpGet]
        [Route("api/Report/TrackYourProgressByTopic")]
        public async Task<ResponseDto> TrackYourProgressByTopic(long topicId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var report = await reportService.TrackYourProgressByTopic(topicId);

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

        [HttpGet]
        [Route("api/Report/GetAssessmentReport")]
        public async Task<ResponseDto> GetAssessmentReport(long assessmentId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetAssessmentConfig");
                var report = await reportService.GetAssessmentReport(assessmentId);

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

        [HttpGet]
        [Route("api/Report/TimeSpentAnalysisBySubject")]
        public async Task<ResponseDto> TimeSpentAnalysisBySubject(long subjectId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var report = await reportService.TimeSpentAnalysisBySubject(subjectId);

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

        [HttpGet]
        [Route("api/Report/TimeSpentAnalysisByChapter")]
        public async Task<ResponseDto> TimeSpentAnalysisByChapter(long chapterId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var report = await reportService.TimeSpentAnalysisByChapter(chapterId);

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

        [HttpGet]
        [Route("api/Report/TimeSpentAnalysisByTopic")]
        public async Task<ResponseDto> TimeSpentAnalysisByTopic(long topicId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var report = await reportService.TimeSpentAnalysisByTopic(topicId);

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

        [HttpGet]
        [Route("api/Report/DifficultyLevelAnalysisBySubject")]
        public async Task<ResponseDto> DifficultyLevelAnalysisBySubject(long subjectId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var report = await reportService.DifficultyLevelAnalysisBySubject(subjectId);

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

        [HttpGet]
        [Route("api/Report/DifficultyLevelAnalysisByChapter")]
        public async Task<ResponseDto> DifficultyLevelAnalysisByChapter(long chapterId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var report = await reportService.DifficultyLevelAnalysisByChapter(chapterId);

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

        [HttpGet]
        [Route("api/Report/DifficultyLevelAnalysisByTopic")]
        public async Task<ResponseDto> DifficultyLevelAnalysisByTopic(long topicId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var report = await reportService.DifficultyLevelAnalysisByTopic(topicId);

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
        //[Route("api/Report/GetTimeSpentAnalysisReport")]
        //public async Task<ResponseDto> GetTimeSpentAnalysisReport(long subjectId)
        //{
        //    ResponseDto responseDto = new ResponseDto();
        //    try
        //    {
        //        var report = await reportService.GetTimeSpentAnalysisReport(subjectId);

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

        //[HttpGet]
        //[Route("api/Report/GetDifficultyLevelAnalysisReport")]
        //public async Task<ResponseDto> GetDifficultyLevelAnalysisReport(long subjectId)
        //{
        //    ResponseDto responseDto = new ResponseDto();
        //    try
        //    {
        //        var report = await reportService.GetDifficultyLevelAnalysisReport(subjectId);

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


        #region OldReports

        //[HttpGet]
        //[Route("api/Report/GetSubjectsReport")]
        //public async Task<ResponseDto> GetSubjectsReport()
        //{
        //    ResponseDto responseDto = new ResponseDto();
        //    try
        //    {
        //        logger.LogInformation("Requested GetSubjectsReport");
        //        var report = await reportService.GetSubjectsReport();

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

        ////[HttpGet]
        ////[Route("api/Report/GetStudentReport")]
        ////public async Task<ResponseDto> GetStudentReport()
        ////{
        ////    ResponseDto responseDto = new ResponseDto();
        ////    try
        ////    {
        ////        logger.LogInformation("Requested GetStudentReport");
        ////        var report = await reportService.GetStudentReport();

        ////        if (report != null)
        ////        {
        ////            responseDto.Result = report;
        ////            responseDto.IsSuccess = true;
        ////        }
        ////        else
        ////        {
        ////            responseDto.IsSuccess = false;
        ////            responseDto.Message = "Not Found";
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        responseDto.IsSuccess = false;
        ////        responseDto.Message = ex.Message;
        ////    }
        ////    return responseDto;
        ////}

        //[HttpGet]
        //[Route("api/Report/GetSubjectReport")]
        //public async Task<ResponseDto> GetSubjectReport(long id)
        //{
        //    ResponseDto responseDto = new ResponseDto();
        //    try
        //    {
        //        logger.LogInformation("Requested GetSubjectReport");
        //        var report = await reportService.GetSubjectReport(id);

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

        #endregion
    }
}
