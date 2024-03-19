﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResoClassAPI.DTOs;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities.Interfaces;
using ResoClassAPI.Utilities;
using ResoClassAPI.Services;

namespace ResoClassAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassService classService;
        private readonly ILogger<ClassController> logger;
        private readonly IExcelReader excelReader;

        public ClassController(IClassService _classService, ILogger<ClassController> _logger, IExcelReader _excelReader)
        {
            classService = _classService;
            logger = _logger;
            excelReader = _excelReader;
        }


        [HttpGet]
        [Authorize(Policy = "Admin")]
        [Route("api/Class/Get")]
        public async Task<ResponseDto> Get(int Id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetUser");
                var chapter = await classService.GetClass(Id);

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
        [Route("api/Class/GetAll")]
        public async Task<ResponseDto> GetAll()
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested GetAllCourses");
                var users = await classService.GetAllClasses();

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
        [Authorize(Policy = "Admin")]
        [Route("api/Class/Create")]
        public async Task<ResponseDto> Post(ClassDto requestDto)
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
                long newId = await classService.CreateClass(requestDto);
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
        [Route("api/Class/Upload")]
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

                    List<ClassDto> subjects = await excelReader.ReadClassesFromExcel(stream);
                    isDataUploaded = await classService.InsertClassesAndLinkToCourses(subjects);
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
        [Route("api/Class/Update/{id}")]
        public async Task<ResponseDto> Put(int id, ClassDto requestDto)
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

                if (await classService.UpdateClass(requestDto))
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
        [Route("api/Class/Delete/{id}")]
        public async Task<ResponseDto> Delete(int id)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                bool result = await classService.DeleteClass(id);

                if (!result)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Not Found";
                }
                else
                {
                    responseDto.Result = "Course Deleted Successfully";
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