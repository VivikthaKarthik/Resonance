namespace ResoClassAPI.Services.Interfaces
{
    public interface ILoggerService
    {
        Task<string> Error(Type entityType, string message, string stackTrace, string exceptionType);
        Task<string> Info(Type entityType, string message, string detailedMessage);

    }
}
