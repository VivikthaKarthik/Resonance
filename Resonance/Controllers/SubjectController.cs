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
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : Controller
    {
        private readonly ISubjectService subjectService;
        private readonly ILogger<SubjectController> logger;
        private readonly IExcelReader excelReader;

        public SubjectController(ISubjectService _subjectService, ILogger<SubjectController> _logger, IExcelReader _excelReader)
        {
            subjectService = _subjectService;
            logger = _logger;
            excelReader = _excelReader;
        }


        [HttpGet]
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
        [Route("GetSubjectsWithCourseId")]
        public async Task<ResponseDto> GetSubjectsWithCourseId(long courseId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetUser");
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

        [HttpGet]
        [Route("GetAll")]
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
        public async Task<ResponseDto> Post(SubjectDto requestDto)
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

        [HttpPost("Upload")]
        public async Task<ResponseDto> UploadExcel(IFormFile file)
        {
            ResponseDto responseDto = new ResponseDto();
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

                if (await excelReader.BulkUpload(file, SqlTableName.Subject))
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

        [HttpPut("{id}")]
        public async Task<ResponseDto> Put(int id, SubjectDto requestDto)
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



        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
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

    }
}

