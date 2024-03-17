using ResoClassAPI.DTOs;
using ResoClassAPI.Services.Interfaces;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ResoClassAPI.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly ILoggerService _loggerService;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, ILoggerService loggerService)
        {
            _logger = logger;
            _loggerService = loggerService;
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
                _loggerService.Error(typeof(GlobalExceptionHandler), ex.Message, ex.StackTrace, ex.GetType().Name);
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
