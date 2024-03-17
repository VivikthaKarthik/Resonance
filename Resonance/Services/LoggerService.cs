
using Microsoft.EntityFrameworkCore;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;

namespace ResoClassAPI.Services
{
    public class LoggerService : ILoggerService
    {
        private IConfiguration config;
        private readonly ResoClassContext dbContext;
        public LoggerService(ResoClassContext _dbContext, IConfiguration configuration)
        {
            config = configuration;
            this.dbContext = _dbContext;
        }

        public async Task<string> Error(Type entityType, string message, string stackTrace, string exceptionType)
        {
            string referenceNumber = string.Empty;
            try
            {
                referenceNumber = Guid.NewGuid().ToString();
                Logger log = new Logger();
                log.ReferenceNumber = referenceNumber;
                log.Message = message;
                log.LogType = "Error";
                log.StackTrace = stackTrace;
                log.EntityName = "";
                log.ExceptionType = exceptionType;
                log.CreateOn = DateTime.Now;

                dbContext.Loggers.Add(log);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return referenceNumber;
        }

        public async Task<string> Info(Type entityType, string message, string detailedMessage)
        {
            string referenceNumber = string.Empty;
            try
            {
                referenceNumber = Guid.NewGuid().ToString();
                Logger log = new Logger();
                log.ReferenceNumber = referenceNumber;
                log.Message = message;
                log.LogType = "Info";
                log.StackTrace = detailedMessage;
                log.EntityName = entityType.Name;
                log.ExceptionType = null;
                log.CreateOn = DateTime.Now;

                dbContext.Loggers.Add(log);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return referenceNumber;
        }
    }
}
