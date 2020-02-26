namespace Example.Services.LoggingService
{
    public interface ILogger
    {
        void Error(string message);

        void Warn(string message);
        
        void Debug(string message);
        
        void UserAction(string message);
    }
}