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
    public class HomeController : ControllerBase
    {
        private readonly IHomeService homeService;
        private readonly ILogger<HomeController> logger;
        private readonly IExcelReader excelReader;

        public HomeController(IHomeService _homeService, ILogger<HomeController> _logger)
        {
            homeService = _homeService;
            logger = _logger;
        }

        [HttpGet]
        [Route("api/Home/SearchItems")]
        public async Task<ResponseDto> SearchItems(string text)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested SearchItems");
                var searchItems = await homeService.SearchItems(text);

                if (searchItems != null)
                {
                    responseDto.Result = searchItems;
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
        [Route("api/Home/GetListItems")]
        public async Task<ResponseDto> GetListItems(string itemName, string? parentName, long? parentId)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                logger.LogInformation("Requested SearchItems");
                var searchItems = await homeService.GetListItems(itemName, parentName, parentId);

                if (searchItems != null)
                {
                    responseDto.Result = searchItems;
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
