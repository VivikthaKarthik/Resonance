using ResoClassAPI.DTOs;
using ResoClassAPI.Services.Interfaces;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ResoClassAPI.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly ICommonService _commonService;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, ICommonService commonService)
        {
            _logger = logger;
            _commonService = commonService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _commonService.LogError(typeof(GlobalExceptionHandler), ex.Message, ex.StackTrace, ex.GetType().Name);
                ResponseDto response = new ResponseDto()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                string responseJson = JsonSerializer.Serialize(response);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(responseJson);
            }
        }
    }
}
